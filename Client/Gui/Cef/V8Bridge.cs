﻿using RDRN_Module;
using System;
using System.Collections.Generic;
using Xilium.CefGlue;

namespace RDRN_Core.Gui.Cef
{
    internal class V8Bridge : CefV8Handler
    {
        private CefBrowser _browser;

        public V8Bridge(CefBrowser browser)
        {
            _browser = browser;
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            Browser father = null;

            LogManager.WriteLog(LogLevel.Trace, "-> Entering JS Execute. Func: " + name + " arg len: " + arguments.Length);

            father = CefUtil.GetBrowserFromCef(_browser);

            if (father == null)
            {
                LogManager.Exception("CEF NO FATHER FOUND FOR BROWSER " + _browser.Identifier);
                returnValue = CefV8Value.CreateNull();
                exception = "NO FATHER WAS FOUND.";
                return false;
            }
            LogManager.WriteLog(LogLevel.Trace, "-> Father was found!");
            try
            {
                switch (name)
                {
                    case "resourceCall":
                        {
                            LogManager.WriteLog(LogLevel.Trace, "-> Entering resourceCall...");

                            List<object> args = new List<object>();

                            for (int i = 1; i < arguments.Length; i++)
                            {
                                args.Add(arguments[i].GetValue());
                            }

                            LogManager.WriteLog(LogLevel.Trace, "-> Executing callback...");

                            object output = father.Callback.Call(arguments[0].GetStringValue(), args.ToArray());

                            LogManager.WriteLog(LogLevel.Trace, "-> Callback executed!");

                            returnValue = V8Helper.CreateValue(output);
                            exception = null;
                            return true;
                        }
                    case "resourceEval":
                        {
                            LogManager.WriteLog(LogLevel.Trace, "-> Entering resource eval");
                            object output = father.Callback.Eval(arguments[0].GetStringValue());
                            LogManager.WriteLog(LogLevel.Trace, "-> callback executed!");

                            returnValue = V8Helper.CreateValue(output);
                            exception = null;
                            return true;
                        }
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "EXECUTE JS FUNCTION");
            }

            returnValue = CefV8Value.CreateNull();
            exception = "";
            return false;
        }
    }
}