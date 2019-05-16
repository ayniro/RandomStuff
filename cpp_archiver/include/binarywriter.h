//
// Created by nick on 16.03.19.
//

#ifndef BINARYWRITER_H
#define BINARYWRITER_H

#include <string>
#include <fstream>
#include <algorithm>
#include <iostream>

class BinaryWriter {
public:
	explicit BinaryWriter(size_t bufferSize = 4, const std::string& path = "", bool append = false);
	~BinaryWriter();

	BinaryWriter(const BinaryWriter&) = delete;
	BinaryWriter& operator=(const BinaryWriter&) = delete;

public:
	bool setFile(const std::string &path, bool append = false);
	bool isGood() const;
	std::string getFilePath() const;
	void showBuf();

	BinaryWriter& operator<<(bool bit);
	void writeByte(uint8_t byte);
	void writeUInt64(uint64_t value);
	void writeUInt32(uint32_t value);
	void writeUInt16(uint16_t value);
	void flushWriter();
	void setBufWritePosition(size_t pos);
	void setBufBit(size_t index, bool value);

private:
	void addBitToBuf(bool bit);

private:
	size_t _bufferSize;
	std::string _filePath;
	size_t _writePosition;
	uint8_t * _bitsBuffer;
	std::ofstream _outputStream;
};

#endif //BINARYWRITER_H
