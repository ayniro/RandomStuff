#include "compressor.h"
#include <QApplication>

int main(int argc, char *argv[])
{
	QApplication a(argc, argv);
	Compressor w;
	w.show();

	return a.exec();
}
