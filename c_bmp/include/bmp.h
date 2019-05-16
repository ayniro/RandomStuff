#ifndef _BMP_
#define _BMP_
#include <stdint.h>

#pragma pack(push, 1)
typedef struct tagBITMAPFILEHEADER {
    uint16_t    bfType;
    uint32_t    bfSize;
    uint16_t    bfReserved1;
    uint16_t    bfReserved2;
    uint32_t    bfOffBits;
} BIT_FILE_HEAD, *PBIT_FILE_HEAD;

typedef struct tagBITMAPINFOHEADER {
    uint32_t    biSize;
    int32_t     biWidth;
    int32_t     biHeight;
    uint16_t    biPlanes;
    uint16_t    biBitCount;
    uint32_t    biCompression;
    uint32_t    biSizeImage;
    int32_t     biXPelsPerMeter;
    int32_t     biYPelsPerMeter;
    uint32_t    biClrUsed;
    uint32_t    biClrImportant;
} BIT_INFO_HEAD, *PBIT_INFO_HEAD;

typedef struct RGBTRIPLE {
    uint8_t     cBlue;
    uint8_t     cGreen;
    uint8_t     cRed;
} RGB;
#pragma pack(pop)

void load_bmp(FILE * input, RGB* buf, int32_t height, int32_t width);

int save_bmp(RGB* buf, BIT_FILE_HEAD head, BIT_INFO_HEAD info, char* outfile, int32_t width, int32_t height);

void crop(RGB* buf, RGB* new_buf, int32_t width, int32_t height, int32_t x, int32_t y, int32_t w, int32_t h, BIT_FILE_HEAD head, BIT_INFO_HEAD info, BIT_FILE_HEAD *n_head, BIT_INFO_HEAD *n_info);

int rotate(RGB* buf, int32_t width, int32_t height);

#endif
