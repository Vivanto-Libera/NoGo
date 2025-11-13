#ifndef BOARD_H
#define BOARD_H
#include<iostream>
#include<pybind11/pybind11.h>
#include <set>

namespace py = pybind11;

enum Color
{
	EMPTY,
	WHITE,
	BLACK,
};
struct Point
{
	int row;
	int col;
	Point(int x, int y)
	{
		row = x;
		col = y;
	}
};

class GoString
{
public:
	Color color;
	std::set<Point> stones;
	std::set<Point> liberties;

	void removeLiberties(const Point& point)
	{
		liberties.erase(point);
	}
	void addLiberties(const Point& point)
	{
		liberties.insert(point);
	}
	int numLiberties()
	{
		return liberties.size();
	}
	GoString merge(GoString goString);

	GoString(Color color, std::set<Point> stones, std::set<Point> liberties) :color(color), stones(stones), liberties(liberties) {};
	~GoString(){}
};

#endif

