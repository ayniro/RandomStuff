#ifndef IMPROVEDRLE_H
#define IMPROVEDRLE_H

#include <QString>
#include <QFile>
#include <QTextStream>
#include <QSet>

#include <utility>
#include <algorithm>
#include <vector>

class ImprovedRLECompression
{
public:
	ImprovedRLECompression() = delete;
	static std::pair<bool, QString> compressFile(const QString &inputPath, const QString &outputPath);
	static std::pair<bool, QString> decompressFile(const QString &inputPath, const QString &outputPath);
	static std::pair<QString, uint32_t> BurrowsWheelerTransformEncode(const QString& str);
	static QString BurrowsWheelerTransformDecode(const QString& str, uint32_t n);
	static QString moveToFrontEncode(QString alphabet, const QString& str);
	static QString moveToFrontDecode(QString alphabet, const QString& str);
private:
	static QString getAlphabet(const QString& inputPath);
};

#endif // IMPROVEDRLE_H
