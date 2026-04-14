#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BakaTest.Core.MVVM
{
    /// <summary>
    /// ViewModelの基底クラス
    /// </summary>
    /// <remarks>
    /// INotifyPropertyChangedを実装し、プロパティ変更通知とDispose機能を提供します。
    /// すべてのViewModelはこのクラスを継承してください。
    /// </remarks>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// プロパティ値が変更されたときに発生するイベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isDisposed;

        /// <summary>
        /// プロパティ値を設定し、変更があれば通知を発行します
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="field">フィールドの参照</param>
        /// <param name="value">設定する値</param>
        /// <param name="propertyName">プロパティ名（自動取得）</param>
        /// <returns>値が変更された場合true</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// プロパティ変更通知を発行します
        /// </summary>
        /// <param name="propertyName">変更されたプロパティ名</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 複数のプロパティ変更通知を一度に発行します
        /// </summary>
        /// <param name="propertyNames">変更されたプロパティ名の配列</param>
        protected void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        #region IDisposable Implementation

        /// <summary>
        /// リソースを解放します
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースを解放します（派生クラスでオーバーライド可能）
        /// </summary>
        /// <param name="disposing">マネージドリソースを解放する場合true</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // マネージドリソースの解放
                // 派生クラスでオーバーライドして実装
            }

            _isDisposed = true;
        }

        /// <summary>
        /// ViewModelが破棄されているか確認します
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion
    }
}
