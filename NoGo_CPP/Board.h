#ifndef BOARD_H
#define BOARD_H
#include<iostream>
#include<pybind11/pybind11.h>
#include <unordered_set>
#include <array>
#include <vector>
#include<pybind11/stl.h>

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
	bool operator==(const Point& point) const
	{
		return row == point.row && col == point.col;
	}
};
namespace std
{
	template<> struct hash<Point>
	{
		size_t operator()(const Point& point) const noexcept
		{
			size_t h1 = hash<int>()(point.row);
			size_t h2 = hash<int>()(point.col);
			return h1 ^ (h2 << 1);
		}
	};
}

class GoString
{
public:
	Color color;
	std::unordered_set<Point> stones;
	std::unordered_set<Point> liberties;

	void removeLiberties(const Point& point)
	{
		liberties.erase(point);
	}
	void addLiberties(const Point& point)
	{
		liberties.insert(point);
	}
	int numLiberties() const
	{
		return liberties.size();
	}
	GoString merge(GoString goString);
	bool hasPoint(Point point) const
	{
		auto it = stones.find(point);
		return it != stones.end();
	}

	bool operator==(const GoString& string) const
	{
		return color == string.color && stones == string.stones && liberties == string.liberties;
	}
	GoString operator=(const GoString& string)
	{
		return GoString(string);
	}

	GoString(Color color, std::unordered_set<Point>& stones, std::unordered_set<Point>& liberties) :color(color), stones(stones), liberties(liberties) {}
	GoString(const GoString& string)
	{
		color = string.color;
		stones = string.stones;
		liberties = string.liberties;
	}
	~GoString(){}
};
namespace std
{
	template<> struct hash<GoString>
	{
		size_t operator()(const GoString& gs) const noexcept
		{
			size_t hash_result = hash<int>()(gs.color);
			for (const auto& stone : gs.stones)
			{
				hash_result += hash<Point>()(stone);
			}
			for (const auto& lib : gs.liberties)
			{
				hash_result += hash<Point>()(lib);
			}
			return hash_result;
		}
	};
}

class Board
{
public:
	static const int rowDirection[4];
	static const int colDirection[4];

	std::array<std::array<Color, 9>, 9> board;
	Color turn;
	std::unordered_set<GoString> strings;

	std::vector<int> legalMoves();
	void applyMove(int move);
	Color isTerminal();
	py::tuple netwrokInput();

	static int indexToNum(int row, int col)
	{
		return row * 10 + col;
	}

	Board();
	Board(const Board& aBoard);
private:
	const GoString findString(Point point)
	{
		for (auto& s : strings)
		{
			if(s.hasPoint(point))
			{
				return s;
			}
		}
	}
};

#endif

