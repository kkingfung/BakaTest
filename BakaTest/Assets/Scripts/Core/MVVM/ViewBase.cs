#nullable enable
using UnityEngine;
using UnityEngine.UIElements;

namespace BakaTest.Core.MVVM
{
    /// <summary>
    /// UI Toolkit用のView基底クラス
    /// </summary>
    /// <typeparam name="TViewModel">このViewにバインドするViewModelの型</typeparam>
    /// <remarks>
    /// UIDocumentを使用したUI Toolkit画面の基底クラスです。
    /// ViewModelとのデータバインディング、イベント管理、ライフサイクル管理を提供します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public abstract class ViewBase<TViewModel> : MonoBehaviour where TViewModel : ViewModelBase
    {
        private UIDocument? _uiDocument;
        private VisualElement? _root;
        private TViewModel? _viewModel;

        /// <summary>
        /// 現在のViewModel
        /// </summary>
        protected TViewModel? ViewModel => _viewModel;

        /// <summary>
        /// UIDocumentのルート要素
        /// </summary>
        protected VisualElement? Root => _root;

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected virtual void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            
            if (_uiDocument == null)
            {
                Debug.LogError($"[{GetType().Name}] UIDocument component not found!");
                return;
            }

            // UIDocumentの準備が整うまで待つ
            if (_uiDocument.rootVisualElement != null)
            {
                InitializeRoot();
            }
            else
            {
                // rootVisualElementが準備されるのを待つ
                StartCoroutine(WaitForRootElement());
            }
        }

        /// <summary>
        /// ルート要素の準備を待つコルーチン
        /// </summary>
        private System.Collections.IEnumerator WaitForRootElement()
        {
            while (_uiDocument != null && _uiDocument.rootVisualElement == null)
            {
                yield return null;
            }
            
            if (_uiDocument != null)
            {
                InitializeRoot();
            }
        }

        /// <summary>
        /// ルート要素を初期化します
        /// </summary>
        private void InitializeRoot()
        {
            if (_uiDocument == null) return;
            
            _root = _uiDocument.rootVisualElement;
            
            if (_root == null)
            {
                Debug.LogError($"[{GetType().Name}] Root VisualElement is null!");
                return;
            }

            OnRootVisualElementReady(_root);
        }

        /// <summary>
        /// ルート要素の準備ができたときに呼ばれます
        /// </summary>
        /// <param name="root">ルート要素</param>
        /// <remarks>
        /// 派生クラスでオーバーライドして、UI要素の取得やイベント登録を行ってください。
        /// </remarks>
        protected virtual void OnRootVisualElementReady(VisualElement root)
        {
            // 派生クラスで実装
        }

        /// <summary>
        /// ViewModelを設定します
        /// </summary>
        /// <param name="viewModel">設定するViewModel</param>
        protected void SetViewModel(TViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                UnbindViewModel();
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                BindViewModel(_viewModel);
            }
        }

        /// <summary>
        /// ViewModelをバインドします
        /// </summary>
        /// <param name="viewModel">バインドするViewModel</param>
        /// <remarks>
        /// 派生クラスでオーバーライドして、ViewModelのイベント購読などを行ってください。
        /// </remarks>
        protected virtual void BindViewModel(TViewModel viewModel)
        {
            // 派生クラスで実装
        }

        /// <summary>
        /// ViewModelのバインドを解除します
        /// </summary>
        /// <remarks>
        /// 派生クラスでオーバーライドして、ViewModelのイベント購読解除などを行ってください。
        /// </remarks>
        protected virtual void UnbindViewModel()
        {
            // 派生クラスで実装
        }

        /// <summary>
        /// UI要素をクエリします（型安全）
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="name">要素の名前</param>
        /// <returns>見つかった要素、見つからない場合はnull</returns>
        protected T? Q<T>(string? name = null) where T : VisualElement
        {
            if (_root == null)
            {
                Debug.LogWarning($"[{GetType().Name}] Root element is null. Cannot query element.");
                return null;
            }

            return _root.Q<T>(name);
        }

        /// <summary>
        /// 複数のUI要素をクエリします
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="name">要素の名前（nullの場合は型のみで検索）</param>
        /// <param name="className">クラス名（nullの場合は無視）</param>
        /// <returns>見つかった要素のリスト</returns>
        protected UQueryBuilder<T> QAll<T>(string? name = null, string? className = null) where T : VisualElement
        {
            if (_root == null)
            {
                Debug.LogWarning($"[{GetType().Name}] Root element is null. Cannot query elements.");
                return new UQueryBuilder<T>(new VisualElement()); // 空のクエリを返す
            }

            var query = _root.Query<T>();
            
            if (name != null)
            {
                query = query.Name(name);
            }
            
            if (className != null)
            {
                query = query.Class(className);
            }

            return query;
        }

        /// <summary>
        /// クリーンアップ処理
        /// </summary>
        protected virtual void OnDestroy()
        {
            UnbindViewModel();
            
            if (_viewModel != null)
            {
                _viewModel.Dispose();
                _viewModel = null;
            }
        }
    }
}
