#include <stdlib.h>
#include <stdio.h>
#include "Application.h"
#include "Time.h"

using namespace LeaderEngine;

void Application::start(int width, int height, const char* title, void (*loadCallback)()) 
{
	if (instance)
		return;

	instance = this;

	if (!glfwInit())
		exit(EXIT_FAILURE);

	Time::timeScale = 1;
	this->loadCallback = loadCallback;

	//window creation
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 0);
	glfwWindowHint(GLFW_OPENGL_CORE_PROFILE, 1);
	glfwWindowHint(GLFW_RESIZABLE, 0);

	window = glfwCreateWindow(width, height, title, NULL, NULL);

	if (!window) {
		glfwTerminate();
		exit(EXIT_FAILURE);
	}

	if (!glewInit())
		exit(EXIT_FAILURE);

	glfwMakeContextCurrent(window);
	glfwSwapInterval(0);

	load();

	while (!glfwWindowShouldClose(window)) {
		lastTime = glfwGetTime();

		update();
		render();

		Time::deltaTimeUnscaled = glfwGetTime() - lastTime;
		Time::deltaTime = Time::deltaTimeUnscaled * Time::timeScale;
	}

	//cleanup
	onClosing();

	glfwDestroyWindow(window);
	glfwTerminate();

	exit(EXIT_SUCCESS);
}

void Application::load() 
{
	glEnable(GL_DEPTH_TEST);

	if (loadCallback)
		(*loadCallback)();
}

void Application::update() 
{
	for (auto go : gameObjects)
		if (go->active)
			go->update();
}

void Application::render() 
{
	int width, height;
	glfwGetFramebufferSize(window, &width, &height);

	glViewport(0, 0, width, height);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	for (auto go : gameObjects)
		if (go->active)
			go->render();

	glfwSwapBuffers(window);
	glfwPollEvents();
}

void Application::onClosing() {
	for (auto go : gameObjects)
		go->onClosing();
}
