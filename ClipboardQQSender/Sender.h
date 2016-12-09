#pragma once

namespace ClipboardQQSender
{

	void pushStringToClipboard(HWND hwnd, string source);
	void sendClipboard(HWND hwnd);
	void sendClipboardln(HWND hwnd);
	void sendClipboardAndSumbit(HWND hwnd);
	void sendVoid(HWND hwnd);
	void sendReturn(HWND hwnd);
	void sendSumbit(HWND hwnd);
	void sendSumbit2(HWND hwnd);
}