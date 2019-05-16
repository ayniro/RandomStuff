#ifndef COMPRESSOR_H
#define COMPRESSOR_H

#include <QMainWindow>
#include <QString>
#include <QDir>
#include <QFileDialog>
#include <QFileInfo>
#include <QTextStream>
#include <QDebug>

#include <utility>

#include "naiverlecompression.h"
#include "improvedtextrle.h"
#include "huffmancompression.h"

namespace Ui
{
	class Compressor;
}

class Compressor : public QMainWindow
{
	Q_OBJECT
public:
	explicit Compressor(QWidget *parent = nullptr);
	~Compressor();

private slots:
	void on_browseButton_clicked();
	void on_filePath_currentTextChanged(const QString &arg1);
	void on_runButton_clicked();

private:
	Ui::Compressor *ui;

	void loadPreviewText();
	void clearPreviewText();
	void addEntryToComboBox(const QString &entry);
	bool checkIfFileExists(const QString &filePath);
	int getBytesPerSymbol();
	int getMaxSymbolAmount();
	void runOperation(std::pair<bool, QString> (*func)(const QString&, const QString&));
};

#endif // COMPRESSOR_H
