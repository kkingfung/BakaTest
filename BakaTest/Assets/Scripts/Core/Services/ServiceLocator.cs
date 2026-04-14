#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BakaTest.Core.Services
{
    /// <summary>
    /// サービスロケーターパターンの実装
    /// </summary>
    /// <remarks>
    /// 依存性注入に使用します。GameBootstrapで初期化・登録を行います。
    /// </remarks>
    public class ServiceLocator
    {
        private static ServiceLocator? _instance;
        private readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }
                return _instance;
            }
        }

        /// <summary>
        /// プライベートコンストラクタ（シングルトンパターン）
        /// </summary>
        private ServiceLocator() { }

        /// <summary>
        /// サービスを登録します
        /// </summary>
        /// <typeparam name="T">サービスのインターフェース型</typeparam>
        /// <param name="service">サービスの実装インスタンス</param>
        public void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service {type.Name} is already registered. Replacing...");
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
                Debug.Log($"[ServiceLocator] Service {type.Name} registered successfully.");
            }
        }

        /// <summary>
        /// サービスを取得します
        /// </summary>
        /// <typeparam name="T">取得するサービスのインターフェース型</typeparam>
        /// <returns>サービスのインスタンス、登録されていない場合はnull</returns>
        public T? Get<T>() where T : class
        {
            var type = typeof(T);
            
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }
            
            Debug.LogError($"[ServiceLocator] Service {type.Name} not found! Make sure it's registered in GameBootstrap.");
            return null;
        }

        /// <summary>
        /// サービスを登録解除します
        /// </summary>
        /// <typeparam name="T">登録解除するサービスの型</typeparam>
        public void Unregister<T>() where T : class
        {
            var type = typeof(T);
            
            if (_services.Remove(type))
            {
                Debug.Log($"[ServiceLocator] Service {type.Name} unregistered.");
            }
            else
            {
                Debug.LogWarning($"[ServiceLocator] Service {type.Name} was not registered.");
            }
        }

        /// <summary>
        /// すべてのサービスをクリアします
        /// </summary>
        public void Clear()
        {
            Debug.Log($"[ServiceLocator] Clearing all {_services.Count} services...");
            _services.Clear();
        }

        /// <summary>
        /// サービスが登録されているか確認します
        /// </summary>
        /// <typeparam name="T">確認するサービスの型</typeparam>
        /// <returns>登録されている場合true</returns>
        public bool IsRegistered<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// 登録されているすべてのサービス型を取得します（デバッグ用）
        /// </summary>
        public IEnumerable<Type> GetRegisteredServiceTypes()
        {
            return _services.Keys;
        }
    }
}
