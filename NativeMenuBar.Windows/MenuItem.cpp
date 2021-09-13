#pragma once
#include <windows.h>
#include <string>

#include "MenuItem.h"

MenuItem::MenuItem(HMENU hMenu, const char* name)
{
	menuHandle = hMenu;
	menuName = std::string(name);
}

UINT MenuItem::GetId() const
{
	return (UINT)menuHandle;
}
