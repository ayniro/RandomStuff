//
// Created by nick on 16.03.19.
//

#include "binarywriter.h"

BinaryWriter::BinaryWriter(uint64_t bufferSize, const std::string &path, bool append)
	: _bufferSize(bufferSize), _filePath(path), _writePosition(0)
{
	_bitsBuffer = new uint8_t[_bufferSize];
	std::fill(_bitsBuffer, _bitsBuffer + _bufferSize, 0);
	setFile(_filePath, append);
}

bool BinaryWriter::setFile(const std::string &path, bool append) {
	if (_outputStream.is_open()) {
		_outputStream.close();
	}
	if (append)
		_outputStream.open(path, std::ofstream::app | std::ofstream::binary);
	else
		_outputStream.open(path, std::ofstream::out | std::ofstream::binary);

	return _outputStream.good();
}

BinaryWriter::~BinaryWriter() {
	flushWriter();
	_outputStream.close();
	delete [] _bitsBuffer;
}

void BinaryWriter::showBuf() {
	for (uint32_t i = 0; i < _bufferSize * 8; ++i) {
		std::cout << ((_bitsBuffer[i]) ? 1 : 0) << ' ';
	}
	std::cout << '\n';
}

BinaryWriter& BinaryWriter::operator<<(bool bit) {
	if (_writePosition >= 8 * _bufferSize) {
		flushWriter();
	}
	addBitToBuf(bit);
	return *this;
}

void BinaryWriter::writeByte(uint8_t byte) {
	if (_writePosition > 8 * (_bufferSize - 1)) {
		flushWriter();
	}
	for (int i = 0; i < 8; ++i) {
		addBitToBuf(static_cast<bool>(byte & (1 << i)));
	}
}

// Well it works...
void BinaryWriter::writeUInt64(uint64_t value) {
	uint32_t offset = 8;
	uint8_t b1 = static_cast<uint8_t>(value >> (offset * 7));
	uint8_t b2 = static_cast<uint8_t>(value >> (offset * 6));
	uint8_t b3 = static_cast<uint8_t>(value >> (offset * 5));
	uint8_t b4 = static_cast<uint8_t>(value >> (offset * 4));
	uint8_t b5 = static_cast<uint8_t>(value >> (offset * 3));
	uint8_t b6 = static_cast<uint8_t>(value >> (offset * 2));
	uint8_t b7 = static_cast<uint8_t>(value >> (offset));
	uint8_t b8 = static_cast<uint8_t>(value);
	writeByte(b8);
	writeByte(b7);
	writeByte(b6);
	writeByte(b5);
	writeByte(b4);
	writeByte(b3);
	writeByte(b2);
	writeByte(b1);
}

// Well it works...
void BinaryWriter::writeUInt32(uint32_t value) {
	uint8_t b1 = static_cast<uint8_t>(value >> 24);
	uint8_t b2 = static_cast<uint8_t>(value >> 16);
	uint8_t b3 = static_cast<uint8_t>(value >> 8);
	uint8_t b4 = static_cast<uint8_t>(value);
	writeByte(b4);
	writeByte(b3);
	writeByte(b2);
	writeByte(b1);
}

void BinaryWriter::writeUInt16(uint16_t value) {
	uint8_t b1 = static_cast<uint8_t>(value >> 8);
	uint8_t b2 = static_cast<uint8_t>(value);
	writeByte(b2);
	writeByte(b1);
}

void BinaryWriter::flushWriter() {
	// Add good check

	if (_writePosition > 0) {
		size_t amountOfBytes = (_writePosition - 1) / 8 + 1;
		for (size_t i = 0; i < amountOfBytes; ++i) {
			_outputStream << _bitsBuffer[i];
			_bitsBuffer[i] = 0;
		}
		_outputStream.flush();
		_writePosition = 0;
	}
}

void BinaryWriter::setBufWritePosition(size_t pos) {
	// Add exceptions
	_writePosition = pos;
	if (_writePosition % 8 != 0) {
		for (auto i = _writePosition; i < _writePosition + _writePosition % 8; ++i) {
			setBufBit(i, false);
		}
	}
	for (auto i = _writePosition / 8; i < _bufferSize; ++i) {
		_bitsBuffer[i] = 0;
	}
}

void BinaryWriter::setBufBit(size_t index, bool value) {
	if (value) {
		_bitsBuffer[index / 8] |= (1 << (index % 8));
	} else {
		_bitsBuffer[index / 8] &= ~(1 << (index % 8));
	}
}

bool BinaryWriter::isGood() const {
	return _outputStream.good();
}

std::string BinaryWriter::getFilePath() const {
	return _filePath;
}

void BinaryWriter::addBitToBuf(bool bit) {
	if (bit) {
		setBufBit(_writePosition, bit);
	}
	_writePosition++;
}
