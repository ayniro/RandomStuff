#ifndef NAIVERLECOMPRESSION_H
#define NAIVERLECOMPRESSION_H

#include <QString>
#include <QFile>
#include <QTextStream>

class NaiveRLECompression
{
public:
	NaiveRLECompression() = delete;
	static std::pair<bool, QString> compressFile(const QString &inputPath, const QString &outputPath);
	static std::pair<bool, QString> decompressFile(const QString &inputPath, const QString &outputPath);
};

#endif // NAIVERLECOMPRESSION_H
