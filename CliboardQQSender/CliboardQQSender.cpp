// CliboardQQSender.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "CliboardQQSender.h"

namespace CliboardQQSender 
{
	extern void pushStringToClipboard(HWND hwnd,string source);
	extern void sendClipboardMessage(HWND hwnd);
}



// 这是导出函数的一个示例。
CLIBOARDQQSENDER_API void Send(HWND value)
{
	CliboardQQSender::sendClipboardMessage(value);
}