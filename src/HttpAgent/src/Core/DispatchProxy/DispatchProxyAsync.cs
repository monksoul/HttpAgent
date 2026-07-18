// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable

using static System.Reflection.AsyncDispatchProxyGenerator;

namespace System.Reflection;

public abstract class DispatchProxyAsync
{
    /// <summary>
    ///     创建指定接口的代理实例
    /// </summary>
    /// <typeparam name="T">要代理的接口类型</typeparam>
    /// <typeparam name="TProxy">
    ///     <see cref="DispatchProxyAsync" />
    /// </typeparam>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    public static T Create<T, TProxy>() where TProxy : DispatchProxyAsync =>
        (T)CreateProxyInstance(typeof(TProxy), typeof(T));

    /// <summary>
    ///     创建指定接口的代理实例
    /// </summary>
    /// <param name="type">要代理的接口类型</param>
    /// <param name="proxyType">
    ///     <see cref="DispatchProxyAsync" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    public static object Create(Type type, Type proxyType) =>
        CreateProxyInstance(proxyType, type);

    /// <summary>
    ///     同步方法调用入口
    /// </summary>
    /// <param name="method">被调用的方法信息</param>
    /// <param name="args">方法参数数组</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    public abstract object Invoke(MethodInfo method, object[] args);

    /// <summary>
    ///     返回 <see cref="Task" /> 的异步方法调用入口
    /// </summary>
    /// <param name="method">被调用的方法信息</param>
    /// <param name="args">方法参数数组</param>
    /// <returns>
    ///     <see cref="Task" />
    /// </returns>
    public abstract Task InvokeAsync(MethodInfo method, object[] args);

    /// <summary>
    ///     返回 <see cref="Task{TResult}" /> 的异步方法调用入口
    /// </summary>
    /// <typeparam name="T">异步操作的返回值类型</typeparam>
    /// <param name="method">被调用的方法信息</param>
    /// <param name="args">方法参数数组</param>
    /// <returns>
    ///     <see cref="Task{TResult}" />
    /// </returns>
    public abstract Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args);

    /// <summary>
    ///     返回 <see cref="ValueTask" /> 的异步方法调用入口
    /// </summary>
    /// <param name="method">被调用的方法信息</param>
    /// <param name="args">方法参数数组</param>
    /// <returns>
    ///     <see cref="ValueTask" />
    /// </returns>
    public abstract ValueTask InvokeValueTaskAsync(MethodInfo method, object[] args);

    /// <summary>
    ///     返回 <see cref="ValueTask{TResult}" /> 的异步方法调用入口
    /// </summary>
    /// <typeparam name="T">异步操作的返回值类型</typeparam>
    /// <param name="method">被调用的方法信息</param>
    /// <param name="args">方法参数数组</param>
    /// <returns>
    ///     <see cref="ValueTask{TResult}" />
    /// </returns>
    public abstract ValueTask<T> InvokeValueTaskAsyncT<T>(MethodInfo method, object[] args);
}