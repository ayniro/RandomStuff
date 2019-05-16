#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "bmp.h"

int main(int argc, char* argv[]) {

    char* action = argv[1];
    char* infile = argv[2];
    char* outfile = argv[3];
    char* x = argv[4];
    char* y = argv[5];
    char* w = argv[6];
    char* h = argv[7];

    if (!(action && infile && outfile && x && y && w && h)) return 1;

    int32_t nx = atoi(x);
    int32_t ny = atoi(y);
    int32_t nw = atoi(w);
    int32_t nh = atoi(h);
    if (nx < 0 || ny < 0 || nw < 0 || nh < 0) return 1;

    BIT_FILE_HEAD head, n_head;
    BIT_INFO_HEAD info, n_info;

    if (strcmp(action, "crop-rotate") == 0){

        FILE * input = fopen(infile, "rb");

        if (input == NULL) {
            return 1;
        }

        //BITMAPFILEHEADER
        fread(&head, 14, 1, input);

        //BITMAPINFOHEADER
        fread(&info, 40, 1, input); 

        if (nx + nw > info.biWidth) return 1;
        if (ny + nh > info.biHeight) return 1;

        RGB * buf = (RGB*)malloc(sizeof(RGB) * info.biHeight * info.biWidth);
        if (buf == NULL) return 1;

        load_bmp(input, buf, info.biHeight, info.biWidth);

        RGB * new_buf = (RGB*)malloc(sizeof(RGB) * nw * nh);
        if (new_buf == NULL) return 1;

        crop(buf, new_buf, info.biWidth, info.biHeight, nx, ny, nw, nh, head, info, &n_head, &n_info);

        if (rotate(new_buf, nw, nh)) return 1;

        if (save_bmp(new_buf, n_head, n_info, outfile, nh, nw)) return 1;

        free(new_buf);

        free(buf);

        fclose(input);
    } else {
        return 1;
    }
    return 0;
}
