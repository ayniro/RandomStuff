#include "improvedtextrle.h"

// WARNING Bad code, just tests of concepts


std::pair<bool, QString> ImprovedRLECompression::compressFile(const QString &inputPath, const QString &outputPath)
{
	QFile inputFile(inputPath);
	QFile outputFile(outputPath);
	QFile alphabetFile(outputPath + ".abc");
	QFile bwtInfoFile(outputPath + ".bwt");
	if (!inputFile.open(QFile::ReadOnly | QFile::Text)) {
		return std::make_pair(false, "Input File open error");
	}
	if (!outputFile.open(QFile::WriteOnly | QFile::Truncate)) {
		inputFile.close();
		return std::make_pair(false, "Output File open error");
	}
	if (!alphabetFile.open(QFile::WriteOnly | QFile::Truncate)) {
		inputFile.close();
		outputFile.close();
		return std::make_pair(false, "Alphabet File open error");
	}
	if (!bwtInfoFile.open(QFile::WriteOnly | QFile::Truncate)) {
		inputFile.close();
		outputFile.close();
		alphabetFile.close();
		return std::make_pair(false, "BWT Info File open error");
	}

	QTextStream inputStream(&inputFile);
	QTextStream outputStream(&outputFile);
	QTextStream alphabetStream(&alphabetFile);
	QTextStream bwtInfoStream(&bwtInfoFile);

	QString inputStr = inputStream.readAll();
	QString alphabet;
	try {
		alphabet = getAlphabet(inputPath);
	} catch (std::exception e) {
		inputFile.close();
		outputFile.close();
		alphabetFile.close();
		bwtInfoFile.close();
		return std::make_pair(false, "Alphabet Read error");
	}
	auto bwt = BurrowsWheelerTransformEncode(inputStr);
	QString mtf = moveToFrontEncode(alphabet, bwt.first);

	outputStream << mtf;
	alphabetStream << alphabet;
	bwtInfoStream << bwt.second;

	inputFile.close();
	outputFile.close();
	alphabetFile.close();
	bwtInfoFile.close();
	return std::make_pair(true, "Operation Successful");
}

std::pair<bool, QString> ImprovedRLECompression::decompressFile(const QString &inputPath, const QString &outputPath)
{
	QFile inputFile(inputPath);
	QFile outputFile(outputPath);
	QFile alphabetFile(inputPath + ".abc");
	QFile bwtInfoFile(inputPath + ".bwt");
	if (!inputFile.open(QFile::ReadOnly | QFile::Text)) {
		return std::make_pair(false, "Output File open error");
	}
	if (!outputFile.open(QFile::WriteOnly | QFile::Truncate)) {
		inputFile.close();
		return std::make_pair(false, "Input File open error");
	}
	if (!alphabetFile.open(QFile::ReadOnly | QFile::Text)) {
		inputFile.close();
		outputFile.close();
		return std::make_pair(false, "Alphabet File open error");
	}
	if (!bwtInfoFile.open(QFile::ReadOnly | QFile::Text)) {
		inputFile.close();
		outputFile.close();
		alphabetFile.close();
		return std::make_pair(false, "BWT Info File open error");
	}

	QTextStream inputStream(&inputFile);
	QTextStream outputStream(&outputFile);
	QTextStream alphabetStream(&alphabetFile);
	QTextStream bwtInfoStream(&bwtInfoFile);

	QString alphabet = alphabetStream.readAll();
	uint32_t bwtInt = static_cast<uint32_t>(bwtInfoStream.readAll().toInt());

	QString mtfDecoded = moveToFrontDecode(alphabet, inputStream.readAll());
	QString decoded = BurrowsWheelerTransformDecode(mtfDecoded, bwtInt);

	outputStream << decoded;

	inputFile.close();
	outputFile.close();
	alphabetFile.close();
	bwtInfoFile.close();
	return std::make_pair(true, "Operation Successful");
}

std::pair<QString, uint32_t> ImprovedRLECompression::BurrowsWheelerTransformEncode(const QString &str)
{
	std::vector<QString> matrix;
	QString currString = str;

	for (int i = 0; i < str.size(); i++) {
		matrix.emplace_back(currString);
		std::rotate(currString.begin(), currString.begin() + 1, currString.end());
	}

	std::sort(matrix.begin(), matrix.end());

	QString outputString;
	uint32_t initialStringIndex = 0;
	for (uint32_t i = 0; i < static_cast<uint32_t>(str.size()); ++i) {
		if (matrix[i] == str) {
			initialStringIndex = i;
		}
		outputString.push_back(matrix[i].back());
	}

	return {outputString, initialStringIndex};
}

QString ImprovedRLECompression::BurrowsWheelerTransformDecode(const QString &str, uint32_t n)
{
	std::vector<std::pair<QChar, uint32_t>> chars;
	chars.reserve(static_cast<uint32_t>(str.size()));
	QString initialString;

	for (int i = 0; i < str.size(); i++) {
		chars.emplace_back(std::make_pair(str[i], i));
	}
	std::sort(chars.begin(), chars.end());

	for (int i = 0; i < str.size(); i++) {
		auto charPair = chars[n];
		n = charPair.second;
		initialString.push_back(charPair.first);
	}

	return initialString;
}

QString ImprovedRLECompression::moveToFrontEncode(QString alphabet, const QString& str)
{
	QString res = "";
	int index = 0;
	for (int i = 0; i < str.size(); ++i) {
		index = alphabet.indexOf(str[i]);
		if (index >= 0 && index <= 9) {
			res += QString::number(index);
		} else {
			if (*(res.end() - 1) != ' ')
				res += " ";
			res += QString::number(index);
			res += " ";
		}
		alphabet.remove(index, 1);
		alphabet.push_front(str[i]);
	}
	return res.trimmed();
}

QString ImprovedRLECompression::moveToFrontDecode(QString alphabet, const QString &str)
{
	QString res = "";
	QChar charToWrite;
	int index = 0;
	int spaceIndex = 0;
	for (int i = 0; i < str.size(); ++i) {
		if (str[i] == ' ') {
			spaceIndex = str.indexOf(' ', i + 1);
			if (spaceIndex == -1)
				spaceIndex = str.size();
			index = str.mid(i + 1, spaceIndex - i - 1).toInt();
			i += spaceIndex - i;
		} else {
			index = QString(str[i]).toInt();
		}
		charToWrite = alphabet[index];
		res += charToWrite;
		alphabet.remove(index, 1);
		alphabet.push_front(charToWrite);
	}
	return res;
}

QString ImprovedRLECompression::getAlphabet(const QString &inputPath)
{
	QSet<QChar> alphabet;
	QFile inputFile(inputPath);
	if (!inputFile.open(QFile::ReadOnly | QFile::Text)) {
		throw new std::runtime_error("Input file open error");
	}
	QTextStream inputStream(&inputFile);

	QChar readChar = inputStream.read(1)[0];
	while (!readChar.isNull()) {
		alphabet += readChar;
		readChar = inputStream.read(1)[0];
	}

	QString alphabetStr = "";
	for (auto c : alphabet) {
		alphabetStr.push_back(c);
	}

	inputFile.close();
	return alphabetStr;
}
