// CppPlayground.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include "test_url_parse.h"

int main()
{
	//vlc_UrlParseInner(new vlc_url_t(), "smb://Michael:qwe123@ewq@192.168.10.247/Nsfw/4K.ts");
	smb2_parse_url("smb://Michael:qwe123@ewq@192.168.10.247/Nsfw/4K.ts");
	system("pause");
}
