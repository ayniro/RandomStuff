#include <stdio.h>
#include <stdlib.h>
#include "bmp.h"

void load_bmp(FILE * input, RGB* buf, int32_t height, int32_t width){
    int emp = width % 4;

    for (int i = 0; i < height; i++){
        for (int j = 0; j < width; j++){
            fread(&buf[j + i * width], 3, 1, input);
        }
        fseek(input, emp, SEEK_CUR);
    }
}

int save_bmp(RGB* buf, BIT_FILE_HEAD head, BIT_INFO_HEAD info, char* outfile, int32_t width, int32_t height){
    BIT_FILE_HEAD h;
    BIT_INFO_HEAD inf;

    uint8_t z = 0;

    h.bfType = head.bfType;
    h.bfSize = 54 + height * (width / 4 * 4);
    h.bfReserved1 = head.bfReserved1;
    h.bfReserved2 = head.bfReserved2;
    h.bfOffBits = 54;

    inf.biSize = sizeof(BIT_INFO_HEAD);
    inf.biWidth = width;
    inf.biHeight = height;
    inf.biPlanes = info.biPlanes;
    inf.biBitCount = info.biBitCount;
    inf.biCompression = info.biCompression;
    inf.biSizeImage = width * height * 3;
    inf.biXPelsPerMeter = info.biXPelsPerMeter;
    inf.biYPelsPerMeter = info.biYPelsPerMeter;
    inf.biClrUsed = info.biClrUsed;
    inf.biClrImportant = info.biClrImportant;

    FILE * output = fopen(outfile, "wb");
    if (output == NULL) return 1;

    fwrite(&h, 14, 1, output);    
    fwrite(&inf, 40, 1, output);

    for (int i = 0; i < height; i++){
        for (int j = 0; j < width; j++){
            fwrite(&buf[j + i * width], 3, 1, output);
        }
        fwrite(&z, 1, width % 4, output);
    }

    fclose(output);

    return 0;
}

void crop(RGB* buf, RGB* new_buf, int32_t width, int32_t height, int32_t x, int32_t y, int32_t w, int32_t h, BIT_FILE_HEAD head, BIT_INFO_HEAD info, BIT_FILE_HEAD *n_head, BIT_INFO_HEAD *n_info){

    int32_t first = width * (height - 1) + x - y * width - (h - 1) * width;
    int32_t pos = first;

    n_head->bfType = head.bfType;
    n_head->bfSize = 54 + h * (w / 4 * 4);
    n_head->bfReserved1 = head.bfReserved1;
    n_head->bfReserved2 = head.bfReserved2;
    n_head->bfOffBits = 54;

    n_info->biSize = sizeof(BIT_INFO_HEAD);
    n_info->biWidth = w;
    n_info->biHeight = h;
    n_info->biPlanes = info.biPlanes;
    n_info->biBitCount = info.biBitCount;
    n_info->biCompression = info.biCompression;
    n_info->biSizeImage = w * h * 3;
    n_info->biXPelsPerMeter = info.biXPelsPerMeter;
    n_info->biYPelsPerMeter = info.biYPelsPerMeter;
    n_info->biClrUsed = info.biClrUsed;
    n_info->biClrImportant = info.biClrImportant;

    for (int i = 0; i < h; i++){
        for (int j = 0; j < w; j++){
            new_buf[j + i * w] = buf[pos];
            pos++;
        }
        pos = first + (i + 1) * width;
    }
}

int rotate(RGB* buf, int32_t width, int32_t height){
    int32_t pos = width - 1;
    int32_t first = pos;

    RGB * new_buf = (RGB*)malloc(sizeof(RGB) * width * height);
    if (new_buf == NULL) return 1;

    for (int i = 0; i < width; i++){
        for (int j = 0; j < height; j++){
            new_buf[j + i * height] = buf[pos];
            pos += width;
        }
        pos = first - 1;
        first--;
    }

    for (int i = 0; i < width * height; i++) buf[i] = new_buf[i];

    free(new_buf);

    return 0;
}
