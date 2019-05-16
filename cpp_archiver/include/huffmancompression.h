#ifndef HUFFMANCOMPRESSION_H
#define HUFFMANCOMPRESSION_H

#include <QString>
#include <QFile>
#include <QTextStream>
#include <QDataStream>
#include <QMap>

#include <queue>
#include <vector>

#include "huffmannode.h"
#include "binarywriter.h"

class HuffmanCompression
{
public:
	HuffmanCompression() = delete;
	static std::pair<bool, QString> compressFile(const QString &inputPath, const QString &outputPath);
	static std::pair<bool, QString> decompressFile(const QString &inputPath, const QString &outputPath);
private:
	static void encodeCharsData(BinaryWriter &br,
							   uint32_t diffCharsAmount,
							   uint32_t textLength,
							   const QMap<QChar, uint32_t> &symbolsAmount);
	static void encodeText(BinaryWriter &br,
						   const QString &inputPath,
						   const QMap<QChar, QBitArray> &huffmanCodes);
	static uint32_t getTextLength(const QMap<QChar, uint32_t> &symbolsMap);
	static QMap<QChar, uint32_t> getSymbolsAmountMap(const QString &inputPath);
	static HuffmanNode* initCharNodes(const QMap<QChar, uint32_t> &symbols, QList<HuffmanNode*> &charNodes);
};

#endif // HUFFMANCOMPRESSION_H
