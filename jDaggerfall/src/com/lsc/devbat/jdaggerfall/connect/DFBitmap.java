package com.lsc.devbat.jdaggerfall.connect;

public class DFBitmap { // Java doesn't have structs, so we have to use a class

	public enum Formats {
		Indexed, ARGB, RGBA, ABGR, BGRA
	}
	
	public Formats format;
	public int width;
	public int height;
	public int stride;
	public byte[] data;
}
