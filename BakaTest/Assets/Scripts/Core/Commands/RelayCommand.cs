#nullable enable
using System;

namespace BakaTest.Core.Commands
{
    /// <summary>
    /// ICommandインターフェース（WPF/Xamarin風）
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// コマンドが実行可能かどうかが変更されたときに発生するイベント
        /// </summary>
        event EventHandler? CanExecuteChanged;

        /// <summary>
        /// コマンドが実行可能かどうかを判定します
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ</param>
        /// <returns>実行可能な場合true</returns>
        bool CanExecute(object? parameter);

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ</param>
        void Execute(object? parameter);
    }

    /// <summary>
    /// デリゲートベースのICommand実装
    /// </summary>
    /// <typeparam name="T">パラメータの型</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Predicate<T?>? _canExecute;

        /// <summary>
        /// コマンドが実行可能かどうかが変更されたときに発生するイベント
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// RelayCommandのコンストラクタ
        /// </summary>
        /// <param name="execute">実行するアクション</param>
        /// <param name="canExecute">実行可能かどうかを判定する述語（省略可能）</param>
        public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが実行可能かどうかを判定します
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ</param>
        /// <returns>実行可能な場合true</returns>
        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            // 型チェックと変換
            if (parameter == null && !typeof(T).IsValueType)
            {
                return _canExecute(default);
            }

            if (parameter is T typedParameter)
            {
                return _canExecute(typedParameter);
            }

            return false;
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ</param>
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            // 型チェックと変換
            if (parameter == null && !typeof(T).IsValueType)
            {
                _execute(default);
            }
            else if (parameter is T typedParameter)
            {
                _execute(typedParameter);
            }
        }

        /// <summary>
        /// CanExecuteChangedイベントを発行します
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// パラメータなしのRelayCommand
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// コマンドが実行可能かどうかが変更されたときに発生するイベント
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// RelayCommandのコンストラクタ
        /// </summary>
        /// <param name="execute">実行するアクション</param>
        /// <param name="canExecute">実行可能かどうかを判定する関数（省略可能）</param>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが実行可能かどうかを判定します
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ（使用されません）</param>
        /// <returns>実行可能な場合true</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="parameter">コマンドのパラメータ（使用されません）</param>
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute();
            }
        }

        /// <summary>
        /// CanExecuteChangedイベントを発行します
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
