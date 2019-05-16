#ifndef HUFFMANNODE_H
#define HUFFMANNODE_H

#include <QChar>
#include <QBitArray>

class HuffmanNode {
public:
	HuffmanNode *left, *right;
	QChar character;
	quint64 frequency;
	qint32 huffmanTreeLayer;

public:
	HuffmanNode(HuffmanNode *left, HuffmanNode *right,
			QChar character, quint64 frequency);
	HuffmanNode* unite(HuffmanNode *other);
	QBitArray getHuffmanCode() const;
	~HuffmanNode();

private:
	QBitArray huffmanCode;
	void addBitToCode(bool bit);
	void increaseTreeLayers(int value = 1);
};

#endif // HUFFMANNODE_H
