#include "huffmannode.h"

HuffmanNode::HuffmanNode(HuffmanNode *left, HuffmanNode *right, QChar character, quint64 frequency)
{
	this->left = left;
	this->right = right;
	this->character = character;
	this->frequency = frequency;
	this->huffmanTreeLayer = 0;
	this->huffmanCode.resize(8);
}

HuffmanNode* HuffmanNode::unite(HuffmanNode *other)
{
	this->increaseTreeLayers(1);
	other->increaseTreeLayers(1);
	this->addBitToCode(false);
	other->addBitToCode(true);
	return new HuffmanNode(this, other, '\0', this->frequency + other->frequency);
}

QBitArray HuffmanNode::getHuffmanCode() const
{
	if (!this->character.isNull()) {
		QBitArray returnArray(this->huffmanCode);
		// reverse to get correct huffman code
		for (int i = 0; i < huffmanTreeLayer / 2; i++) {
			int swapIndex = huffmanTreeLayer - i - 1;
			bool bit = returnArray.testBit(swapIndex);
			returnArray.setBit(swapIndex, returnArray.testBit(i));
			returnArray.setBit(i, bit);
		}
		if (huffmanTreeLayer > 0)
			returnArray.resize(huffmanTreeLayer);
		else
			returnArray.resize(1);
		return returnArray;
	}
	return QBitArray();
}

// Can be rewritten to remove recursion, but whatever
void HuffmanNode::increaseTreeLayers(int value)
{
	huffmanTreeLayer += value;
	if (this->left != nullptr) {
		this->left->increaseTreeLayers(value);
	}
	if (this->right != nullptr) {
		this->right->increaseTreeLayers(value);
	}
}

// Can be rewritten to remove recursion, but whatever
void HuffmanNode::addBitToCode(bool bit)
{
	if (!this->character.isNull()) {
		if (huffmanTreeLayer >= this->huffmanCode.size()) {
			huffmanCode.resize(huffmanCode.size() * 2);
		}
		this->huffmanCode.setBit(huffmanTreeLayer - 1, bit);
	} else {
		if (this->left != nullptr) {
			this->left->addBitToCode(bit);
		}
		if (this->right != nullptr) {
			this->right->addBitToCode(bit);
		}
	}
}

HuffmanNode::~HuffmanNode()
{
	if (left != nullptr) delete left;
	if (right != nullptr) delete right;
}
