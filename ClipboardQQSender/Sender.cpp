#include "stdafx.h"
#include "Sender.h"
namespace ClipboardQQSender
{
	void pushStringToClipboard(HWND hwnd,string source)
	{
		if (hwnd == 0x0)
		{
			return;
		}
		if (OpenClipboard(hwnd))
		{
			HGLOBAL clipbuffer;
			char * buffer;
			EmptyClipboard();
			clipbuffer = GlobalAlloc(GMEM_DDESHARE, source.length() + 1);
			buffer = (char*)GlobalLock(clipbuffer);
			strcpy_s(buffer, source.length() + 1, source.data());
			GlobalUnlock(clipbuffer);
			SetClipboardData(CF_TEXT, clipbuffer);
			CloseClipboard();
		}
	}

	void sendClipboard(HWND hwnd)
	{
		if (hwnd == 0x0)
		{
			return;
		}
		Sleep(500);
		keybd_event(VK_CONTROL, 0x1D, 0, 0);
		Sleep(100);
		PostMessage(hwnd, WM_KEYDOWN, 0x00000056, 0x002F0001);
		Sleep(100);
		PostMessage(hwnd, WM_KEYUP, 0x00000056, 0xC02F0001);
		Sleep(100);
		keybd_event(VK_CONTROL, 0x1D, KEYEVENTF_KEYUP, 0);
	}

	void sendClipboardln(HWND hwnd)
	{
		if (hwnd == 0x0)
		{
			return;
		}
		Sleep(500);
		keybd_event(VK_CONTROL, 0x1D, 0, 0);
		Sleep(100);
		PostMessage(hwnd, WM_KEYDOWN, 0x00000056, 0x002F0001);
		Sleep(100);
		PostMessage(hwnd, WM_KEYUP, 0x00000056, 0xC02F0001);
		Sleep(100);
		keybd_event(VK_CONTROL, 0x1D, KEYEVENTF_KEYUP, 0);
		Sleep(100);
		PostMessage(hwnd, WM_KEYDOWN, 0x0000000D, 0x001C0001);
		Sleep(100);
		PostMessage(hwnd, WM_KEYUP, 0x0000000D, 0xC01C0001);
	}

	void sendClipboardAndSumbit(HWND hwnd)
	{
		if (hwnd == 0x0)
		{
			return;
		}
		Sleep(500);
		keybd_event(VK_CONTROL, 0x1D, 0, 0);
		Sleep(100);
		PostMessage(hwnd, WM_KEYDOWN, 0x00000056, 0x002F0001);
		Sleep(100);
		PostMessage(hwnd, WM_KEYUP, 0x00000056, 0xC02F0001);
		Sleep(100);
		PostMessage(hwnd, WM_KEYDOWN, 0x0000000D, 0x001C0001);
		Sleep(100);
		PostMessage(hwnd, WM_KEYUP, 0x0000000D, 0xC01C0001);
		Sleep(100);
		keybd_event(VK_CONTROL, 0x1D, KEYEVENTF_KEYUP, 0);
	}

	void sendVoid(HWND hwnd)
	{
		if (hwnd == 0x0)
		{
			return;
		}
		Sleep(500);
		PostMessage(hwnd, WM_SYSKEYDOWN, 0x00000011, 0x20380001);
		Sleep(100);
		PostMessage(hwnd, WM_SYSKEYUP, 0x00000011, 0xC0380001);
	}

	void sendSumbit(HWND hwnd)
	{
		if (hwnd == 0x0)
		{
			return;
		}
		Sleep(500);
		keybd_event(VK_CONTROL, 0x1D, 0, 0);
		Sleep(100);
		PostMessage(hwnd, WM_KEYDOWN, 0x0000000D, 0x001C0001);
		Sleep(100);
		PostMessage(hwnd, WM_KEYUP, 0x0000000D, 0xC01C0001);
		Sleep(100);
		keybd_event(VK_CONTROL, 0x1D, KEYEVENTF_KEYUP, 0);
	}
}