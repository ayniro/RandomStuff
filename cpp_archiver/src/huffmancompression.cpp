#include "huffmancompression.h"

#define LOG true
static const uint32_t checkInt32 = 0xefefefef;

std::pair<bool, QString> HuffmanCompression::compressFile(const QString &inputPath,
									  const QString &outputPath)
{
	auto symbolsAmount = getSymbolsAmountMap(inputPath);
	QList<HuffmanNode*> charNodes;
	initCharNodes(symbolsAmount, charNodes);

	// Contains symbols from text with corresponding code
	QMap<QChar, QBitArray> huffmanCodes;
	for (int i = 0; i < charNodes.size(); i++) {
		huffmanCodes[charNodes.at(i)->character] = charNodes.at(i)->getHuffmanCode();
	}

	uint32_t differentCharsAmount = static_cast<uint32_t>(symbolsAmount.size());

	uint32_t textLength = getTextLength(symbolsAmount);

	try {
		BinaryWriter b(1, outputPath.toStdString(), false);
		encodeCharsData(b, differentCharsAmount, textLength, symbolsAmount);
		b.setFile(outputPath.toStdString(), true);
		encodeText(b, inputPath, huffmanCodes);
	} catch (std::exception e) {
		return std::make_pair(false, e.what());
	}

	if (LOG) {
		QFile logFile(outputPath + ".log");
		if (!logFile.open(QFile::WriteOnly | QFile::Truncate)) {
			throw new std::runtime_error("Input Log file open error");
		}
		QTextStream logStream(&logFile);

		auto huffmanCodesIter = huffmanCodes.begin();
		logStream << "Symbols:\n\n";
		auto bitsToStr = [](const char * bits, int length) {
			QString res = "";
			for (int i = 0; i < length; ++i) {
				res += bits[i] ? '1' : '0';
			}
			return res;
		};
		for (; huffmanCodesIter != huffmanCodes.end(); ++huffmanCodesIter) {
			logStream << "Symbol: "
					  << huffmanCodesIter.key()
					  << "; Huffman Code: "
					  << bitsToStr(huffmanCodesIter.value().bits(), huffmanCodesIter.value().size())
					  << "; Frequency: " << symbolsAmount[huffmanCodesIter.key()]
					  << '\n';
		}

		double price = 0;
		auto symbolsIter = symbolsAmount.begin();
		for (; symbolsIter != symbolsAmount.end(); ++symbolsIter) {
			price += (static_cast<double>(symbolsIter.value()) / textLength) * huffmanCodes[symbolsIter.key()].size();
		}
		logStream << "\nCoding price: " << price << '\n';

		QFile in(inputPath);
		QFile out(outputPath);
		if (in.open(QFile::ReadOnly) || out.open(QFile::ReadOnly)) {
			logStream << "Compression ratio: " << static_cast<double>(in.size()) / out.size() << '\n';
			in.close();
			out.close();
		}

		in.close();
		out.close();
		logFile.close();
	}

	return std::make_pair(true, "Operation Successful");
}

// Diff Chars Amount -- 4 bytes
// Text Length -- 4 bytes
// For each char : Char Code -- 2 bytes
//                 Char Frequency -- 4 bytes
void HuffmanCompression::encodeCharsData(BinaryWriter &br,
										 uint32_t diffCharsAmount,
										 uint32_t textLength,
										 const QMap<QChar, uint32_t> &symbolsAmount)
{
	br.writeUInt32(checkInt32);
	br.writeUInt32(diffCharsAmount);
	br.writeUInt32(textLength);

	auto symbolsIter = symbolsAmount.begin();
	for (; symbolsIter != symbolsAmount.end(); ++symbolsIter) {
		br.writeUInt16(symbolsIter.key().unicode());
		br.writeUInt32(static_cast<uint32_t>(symbolsIter.value()));
	}

	br.flushWriter();
}

void HuffmanCompression::encodeText(BinaryWriter &br,
									const QString &inputPath,
									const QMap<QChar, QBitArray> &huffmanCodes)
{
	QFile inputFile(inputPath);
	if (!inputFile.open(QFile::ReadOnly | QFile::Text)) {
		throw new std::runtime_error("Input file open error");
	}
	QTextStream input(&inputFile);
	QChar currChar;
	input >> currChar;

	while (!currChar.isNull()) {
		for (int i = 0; i < huffmanCodes[currChar].size(); ++i) {
			br << huffmanCodes[currChar].at(i);
		}

		input >> currChar;
	}

	br.flushWriter();
}

uint32_t HuffmanCompression::getTextLength(const QMap<QChar, uint32_t>& symbolsMap) {
	uint32_t amount = 0;

	for (uint32_t p : symbolsMap) {
		amount += p;
	}

	return amount;
}

// Repeating code : compressFile
// Redundant code : initCharNodes
// Really bad code ahead
std::pair<bool, QString> HuffmanCompression::decompressFile(const QString &inputPath,
										const QString &outputPath)
{
	uint32_t checkInt32;
	uint32_t diffCharsAmount;
	uint32_t textLength;
	QMap<QChar, uint32_t> symbolsAmount;
	// InputStream doesn't check opened file
	std::ifstream inputStream(inputPath.toStdString(), std::ifstream::in | std::ifstream::binary);

	inputStream.read(reinterpret_cast<char*>(&checkInt32), 4);
	if (checkInt32 != 0xefefefef) {
		inputStream.close();
		return std::make_pair(false, "Incorrect binary file");
	}
	inputStream.read(reinterpret_cast<char*>(&diffCharsAmount), 4);
	inputStream.read(reinterpret_cast<char*>(&textLength), 4);

	for (uint32_t i = 0; i < diffCharsAmount; ++i) {
		QChar charForRead;
		uint32_t charFrequency;
		inputStream.read(reinterpret_cast<char*>(&charForRead), 2);
		inputStream.read(reinterpret_cast<char*>(&charFrequency), 4);
		symbolsAmount[charForRead] = charFrequency;
	}

	QList<HuffmanNode*> charNodes;
	HuffmanNode* root = initCharNodes(symbolsAmount, charNodes);

	// Text Decompression

	QFile outputFile(outputPath);
	if (!outputFile.open(QFile::WriteOnly | QFile::Truncate)) {
		inputStream.close();
		return std::make_pair(false, "Output File open error");
	}
	QTextStream outputStream(&outputFile);

	uint32_t writtenChars = 0;
	uint8_t byteRead;
	uint8_t bitMask = 1;
	QChar charForWrite = '\0';
	HuffmanNode *currNode = root;
	inputStream.read(reinterpret_cast<char*>(&byteRead), 1);

	while (writtenChars != textLength) {
		while (charForWrite.isNull()) {
			if (currNode->character.isNull()) {
				if (static_cast<bool>(byteRead & bitMask)) {
					currNode = currNode->right;
				} else {
					currNode = currNode->left;
				}
				if (bitMask == (1 << 7)) {
					inputStream.read(reinterpret_cast<char*>(&byteRead), 1);
					bitMask = 1;
				} else {
					bitMask <<= 1;
				}
			} else {
				charForWrite = currNode->character;
				currNode = root;
			}
		}

		outputStream << charForWrite;
		charForWrite = '\0';
		++writtenChars;
	}

	inputStream.close();
	outputFile.close();

	return std::make_pair(true, "Operation Successful");
}

QMap<QChar, uint32_t> HuffmanCompression::getSymbolsAmountMap(const QString &inputPath)
{
	QFile inputFile(inputPath);
	if (!inputFile.open(QFile::ReadOnly | QFile::Text)) {
		throw new std::runtime_error("Input file open error");
	}
	QTextStream input(&inputFile);
	QChar currChar;
	QMap<QChar, uint32_t> symbols;

	input >> currChar;
	while (!currChar.isNull()) {
		symbols[currChar]++;
		input >> currChar;
	}

	inputFile.close();

	return symbols;
}

struct CompareNodes
{
	// Greater => min priority queue
	bool operator()(const HuffmanNode* first, const HuffmanNode* second)
	{
		if (first->frequency > second->frequency)
			return true;
		return false;
	}
};

// Generate huffman tree based on symbols
HuffmanNode* HuffmanCompression::initCharNodes(const QMap<QChar, uint32_t> &symbols, QList<HuffmanNode*> &charNodes)
{	
	std::priority_queue<
			HuffmanNode*, std::vector<HuffmanNode*>, CompareNodes> pqueue;
	auto mapIterator = symbols.constBegin();

	while (mapIterator != symbols.constEnd()) {
		HuffmanNode *charNode =
				new HuffmanNode(nullptr, nullptr, mapIterator.key(), mapIterator.value());
		charNodes.append(charNode);
		pqueue.push(charNode);
		++mapIterator;
	}

	while (pqueue.size() > 1) {
		HuffmanNode *firstNode = pqueue.top();
		pqueue.pop();
		HuffmanNode *secondNode = pqueue.top();
		pqueue.pop();
		HuffmanNode *newNode = firstNode->unite(secondNode);
		pqueue.push(newNode);
	}

	HuffmanNode *root = pqueue.top();
	pqueue.pop();

	return root;
}
