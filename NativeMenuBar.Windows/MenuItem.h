#pragma once
#include <windows.h>
#include <string>

class MenuItem
{
public:
	MenuItem(HMENU hMenu, const char* name);

	UINT GetId() const;

	HMENU menuHandle;
	std::string menuName;
};