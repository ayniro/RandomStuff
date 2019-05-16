#include "compressor.h"
#include "ui_compressor.h"

const quint64 MAX_PREVIEW_LENGTH = 4096;

const QString TEXT_NAIVE_RLE_COMP = "Naive Text RLE: Compression";
const QString TEXT_NAIVE_RLE_DECOMP = "Naive Text RLE: Decompression";
const QString TEXT_BWT_MTF_RLE_COMP = "BWT + MTF Text RLE: Compression";
const QString TEXT_BWT_MTF_RLE_DECOMP = "BWT + MTF Text RLE: Decompression";
const QString TEXT_HUFFMAN_COMP = "Text Huffman Algorithm: Compression";
const QString TEXT_HUFFMAN_DECOMP = "Text Huffman Algorithm: Decompression";

Compressor::Compressor(QWidget *parent)
	: QMainWindow(parent), ui(new Ui::Compressor)
{
	ui->setupUi(this);
	ui->algoChooser->addItem(TEXT_NAIVE_RLE_COMP);
	ui->algoChooser->addItem(TEXT_BWT_MTF_RLE_COMP);
	ui->algoChooser->addItem(TEXT_HUFFMAN_COMP);
	ui->algoChooser->addItem(TEXT_NAIVE_RLE_DECOMP);
	ui->algoChooser->addItem(TEXT_BWT_MTF_RLE_DECOMP);
	ui->algoChooser->addItem(TEXT_HUFFMAN_DECOMP);

	ui->algoChooser->setItemData(
				ui->algoChooser->findText(TEXT_NAIVE_RLE_COMP),
				"Doesn't work on files with numbers", Qt::ToolTipRole);

	ui->algoChooser->setItemData(
				ui->algoChooser->findText(TEXT_BWT_MTF_RLE_COMP),
				"O(n^2) of memory, where n is file length. Produces .abc and .bwt additional files", Qt::ToolTipRole);

	ui->algoChooser->setItemData(
				ui->algoChooser->findText(TEXT_HUFFMAN_COMP),
				"Works only for text files", Qt::ToolTipRole);

	ui->algoChooser->setItemData(
				ui->algoChooser->findText(TEXT_NAIVE_RLE_DECOMP),
				"Works only for text files", Qt::ToolTipRole);

	ui->algoChooser->setItemData(
				ui->algoChooser->findText(TEXT_BWT_MTF_RLE_DECOMP),
				".abc and .bwt files are necessary for decompression", Qt::ToolTipRole);

	ui->algoChooser->setItemData(
				ui->algoChooser->findText(TEXT_HUFFMAN_DECOMP),
				"Works only for binary files", Qt::ToolTipRole);
}

Compressor::~Compressor()
{
	delete ui;
}

void Compressor::on_browseButton_clicked()
{
	QString file = QDir::toNativeSeparators(
					   QFileDialog::getOpenFileName(this, tr("Find File"),
					   QDir::currentPath()));

	if (!file.isEmpty()) {
		addEntryToComboBox(file);
		loadPreviewText();
	}
}

void Compressor::loadPreviewText()
{
	if (checkIfFileExists(ui->filePath->currentText())) {
		QFile file(ui->filePath->currentText());
		if (!file.open(QFile::ReadOnly | QFile::Text)) {
			ui->statusBar->showMessage("Unable to Open File");
		} else {
			QTextStream writer(&file);
			ui->statusBar->clearMessage();
			ui->textBrowser->setText(writer.read(MAX_PREVIEW_LENGTH));
		}
	} else {
		ui->statusBar->showMessage("Incorrect File Path");
	}
}

void Compressor::clearPreviewText()
{
	ui->textBrowser->setText("");
}

bool Compressor::checkIfFileExists(const QString &filePath)
{
	QFileInfo checkFile(filePath);
	if (checkFile.exists() && checkFile.isFile()) {
		return true;
	} else {
		return false;
	}
}

void Compressor::on_filePath_currentTextChanged(const QString &arg1)
{
	if (checkIfFileExists(arg1)) {
		ui->statusBar->clearMessage();
		addEntryToComboBox(arg1);
		loadPreviewText();
		ui->newFileName->setText(QFileInfo(arg1).fileName() + ".changed");
	} else {
		clearPreviewText();
		ui->statusBar->showMessage("Incorrect File Path");
	}
}

void Compressor::addEntryToComboBox(const QString &entry) {
	if (ui->filePath->findText(entry) == -1) {
		ui->filePath->addItem(entry);
	}
	ui->filePath->setCurrentIndex(ui->filePath->findText(entry));
}

void Compressor::on_runButton_clicked()
{
	ui->statusBar->showMessage("");
	if (ui->algoChooser->currentText() == TEXT_NAIVE_RLE_COMP) {
		runOperation(&NaiveRLECompression::compressFile);
	} else if (ui->algoChooser->currentText() == TEXT_NAIVE_RLE_DECOMP) {
		runOperation(&NaiveRLECompression::decompressFile);
	} else if (ui->algoChooser->currentText() == TEXT_BWT_MTF_RLE_COMP) {
		runOperation(&ImprovedRLECompression::compressFile);
	} else if (ui->algoChooser->currentText() == TEXT_BWT_MTF_RLE_DECOMP) {
		runOperation(&ImprovedRLECompression::decompressFile);
	} else if (ui->algoChooser->currentText() == TEXT_HUFFMAN_COMP) {
		runOperation(&HuffmanCompression::compressFile);
	} else if (ui->algoChooser->currentText() == TEXT_HUFFMAN_DECOMP) {
		runOperation(&HuffmanCompression::decompressFile);
	}
}

void Compressor::runOperation(std::pair<bool, QString> (*func)(const QString&, const QString&))
{
	if (checkIfFileExists(ui->filePath->currentText())) {
		auto result =
				func(
					ui->filePath->currentText(),
					QFileInfo(ui->filePath->currentText()).absoluteDir().path() + "/" +
					ui->newFileName->text());
		ui->statusBar->showMessage(result.second);
		/*
		if (resultSuccessful.first) {
			ui->statusBar->showMessage("Operation Successful");
		} else {
			ui->statusBar->showMessage("Operation Failed");
		}
		*/
	} else {
		ui->statusBar->showMessage("Incorrect File Path");
	}
}
