// CliboardQQSender.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "CliboardQQSender.h"

namespace CliboardQQSender 
{
	extern HWND g_QQHwnd;
	extern void pushStringToClipboard(string source);
	extern void sendClipboardMessage();
}



// 这是导出函数的一个示例。
CLIBOARDQQSENDER_API void send(void)
{
	CliboardQQSender::sendClipboardMessage();
}

// 这是导出函数的一个示例。
CLIBOARDQQSENDER_API void setHwnd(UINT32 value)
{
	CliboardQQSender::g_QQHwnd = (HWND)value;
}