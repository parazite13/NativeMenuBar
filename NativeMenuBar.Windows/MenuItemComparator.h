#pragma once
#include <string>

#include "MenuItem.h"

struct MenuItemNameComparator
{
public:
	explicit MenuItemNameComparator(std::string name) : menuItemName(name) { }
	inline bool operator()(const MenuItem& m) const { return m.menuName == menuItemName; }
private:
	std::string menuItemName;
};

struct MenuItemIdComparator
{
public:
	explicit MenuItemIdComparator(UINT id) : menuItemId(id) { }
	inline bool operator()(const MenuItem& m) const { return m.GetId() == menuItemId; }
private:
	UINT menuItemId;
};