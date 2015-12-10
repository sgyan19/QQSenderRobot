// CliboardQQSender.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "CliboardQQSender.h"

namespace CliboardQQSender 
{
	extern void pushStringToClipboard(HWND hwnd, string source);
	extern void sendClipboard(HWND hwnd);
	extern void sendClipboardln(HWND hwnd);
	extern void sendClipboardAndSumbit(HWND hwnd);
	extern void sendSumbit(HWND hwnd);
}

// 这是导出函数的一个示例。
CLIBOARDQQSENDER_API void Paste(HWND value)
{
	CliboardQQSender::sendClipboard(value);
}

// 这是导出函数的一个示例。
CLIBOARDQQSENDER_API void Pasteln(HWND value)
{
	CliboardQQSender::sendClipboardln(value);
}

// 这是导出函数的一个示例。
CLIBOARDQQSENDER_API void PasteAndSumbit(HWND value)
{
	CliboardQQSender::sendClipboardAndSumbit(value);
}

// 这是导出函数的一个示例。
CLIBOARDQQSENDER_API void Sumbit(HWND value)
{
	CliboardQQSender::sendSumbit(value);
}