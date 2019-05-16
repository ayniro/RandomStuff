#include "naiverlecompression.h"

std::pair<bool, QString> NaiveRLECompression::compressFile(const QString &inputPath, const QString &outputPath)
{
	QFile inputFile(inputPath);
	QFile outputFile(outputPath);

	if (!inputFile.open(QFile::ReadOnly | QFile::Text)) {
		return std::make_pair(false, "Input File open error");
	}
	if (!outputFile.open(QFile::WriteOnly | QFile::Truncate)) {
		inputFile.close();
		return std::make_pair(false, "Output File open error");
	}

	QTextStream input(&inputFile);
	QTextStream outputStream(&outputFile);
	QChar currChar;
	input >> currChar;
	QChar prevChar = currChar;
	qint64 runLength;

	while (!currChar.isNull()) {
		runLength = 1;
		prevChar = currChar;
		while (!(input >> currChar).atEnd() && currChar == prevChar) {
			runLength++;
		}
		outputStream << QString::number(runLength);
		outputStream << prevChar;
	}

	inputFile.close();
	outputFile.close();
	return std::make_pair(true, "Operation Successful");
}

std::pair<bool, QString> NaiveRLECompression::decompressFile(const QString &inputPath, const QString &outputPath)
{
	QFile inputFile(inputPath);
	QFile outputFile(outputPath);

	if (!inputFile.open(QFile::ReadOnly | QFile::Text)) {
		return std::make_pair(false, "Input File open error");
	}
	if (!outputFile.open(QFile::WriteOnly | QFile::Truncate)) {
		inputFile.close();
		return std::make_pair(false, "Output File open error");
	}

	QTextStream input(&inputFile);
	QTextStream outputStream(&outputFile);
	int runLength;
	QChar symbol;

	while (!(input >> runLength).atEnd()) {
		input >> symbol;
		for (int i = 0; i < runLength; i++) {
			outputStream << symbol;
		}
	}

	inputFile.close();
	outputFile.close();
	return std::make_pair(true, "Operation Successful");
}
