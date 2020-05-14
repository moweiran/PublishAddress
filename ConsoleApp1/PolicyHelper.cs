using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public static class PolicyHelper
    {
        public static void RetryForever(Action action)
        {
            Policy.Handle<Exception>()   //指定需要重试的异常类型
                      .RetryForever((ex, count, context) =>
                      {     //指定发生异常重试的次数
                          Console.WriteLine($"重试次数{count},异常{ex.Message}");
                          Thread.Sleep(100);
                      })
                      .Execute(() =>
                      {
                          action();
                      });
        }
    }
}
