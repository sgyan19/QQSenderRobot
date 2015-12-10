#pragma once

namespace ClipboardQQSender
{

	void pushStringToClipboard(HWND hwnd, string source);
	void sendClipboard(HWND hwnd);
	void sendClipboardln(HWND hwnd);
	void sendClipboardAndSumbit(HWND hwnd);
	void sendSumbit(HWND hwnd);
}