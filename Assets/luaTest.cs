
using UnityEngine;

using System;
using System.Collections;
using System.Runtime.InteropServices;

public class luaTest : MonoBehaviour {

	[DllImport ("__Internal")] public static extern IntPtr luaL_newstate();
	[DllImport ("__Internal")] public static extern void luaL_openlibs(IntPtr lua_State);
	[DllImport ("__Internal")] public static extern void lua_close(IntPtr lua_State);
	[DllImport ("__Internal")] public static extern void lua_pushcclosure(IntPtr lua_State, LuaFunction func, int n);
	[DllImport ("__Internal")] public static extern void lua_setglobal(IntPtr lua_State, string s);
	[DllImport ("__Internal")] public static extern int lua_pcallk(IntPtr lua_State, int nargs, int nresults, int errfunc, int ctx, LuaFunction func);
	[DllImport ("__Internal")] public static extern int luaL_loadfilex(IntPtr lua_State, string s, string mode);
	[DllImport ("__Internal")] public static extern int luaL_loadstring(IntPtr lua_State, string s);
	[DllImport ("__Internal")] public static extern IntPtr luaL_checklstring(IntPtr lua_State, int idx, IntPtr len);

	public delegate int LuaFunction(IntPtr pLuaState);

	public static void lua_register(IntPtr pLuaState, string strFuncName, LuaFunction pFunc)
	{
	    lua_pushcclosure(pLuaState, pFunc, 0);
	    lua_setglobal(pLuaState, strFuncName);
	}

	public static int luaL_dofile(IntPtr lua_State, string s)
	{
	    if (luaL_loadfilex(lua_State, s, null) != 0)
	        return 1;
	 
	    return lua_pcallk(lua_State, 0, -1, 0, 0, null);
	}

	[System.AttributeUsage(System.AttributeTargets.Method)]
	public sealed class MonoPInvokeCallbackAttribute : Attribute {
	    private Type type;
	    public MonoPInvokeCallbackAttribute (Type t) { type =t;}
	}

	[MonoPInvokeCallbackAttribute (typeof (LuaFunction))]
	public static int TestDisplay(IntPtr pLuaState)
	{
	    IntPtr retPtr =luaL_checklstring(pLuaState, 1, IntPtr.Zero);
	    string retStr = Marshal.PtrToStringAnsi(retPtr);
			
	    Debug.Log("This line was plotted by TestDisplay() : msg ="+retStr);

	    return 0;
	}

	public static IntPtr m_luaState = IntPtr.Zero;

	public static string GetAppPath()
	{
	    return Application.dataPath.Substring(0, Application.dataPath.Length-4);
	}	

	// Use this for initialization
	void Start () {
	    m_luaState = luaL_newstate();
	    luaL_openlibs(m_luaState);
	    lua_register(m_luaState, "TestDisplay", TestDisplay);
	    luaL_dofile(m_luaState, GetAppPath() + "LuaScript/lua.txt");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnApplicationQuit()
	{
		lua_close(m_luaState);	
	}

}
