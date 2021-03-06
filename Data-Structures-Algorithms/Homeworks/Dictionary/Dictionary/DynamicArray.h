#pragma once

template<typename Type>
class DynamicArray
{
public:
	DynamicArray(int capacity = 4);

	~DynamicArray();

public:
	int Count();
	void Clear();
	void Add(Type& item);
	void Insert(Type& item, int index);
	Type& ElementAt(int index);
	int IndexOf(Type data);
	bool Contains(Type& item);
	void RemoveAt(int index);
	void Remove(Type& item);

private:
	Type *data;
	int size;
	int capacity;
	void Resize();
};

