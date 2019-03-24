﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using ThanksCardClient.Models;
using ThanksCardClient.Services;

namespace ThanksCardClient.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom    : ViewModelCommand
         *  lvcomn   : ViewModelCommand(CanExecute無)
         *  llcom    : ListenerCommand(パラメータ有のコマンド)
         *  llcomn   : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop    : 変更通知プロパティ
         *  lsprop   : 変更通知プロパティ(ショートバージョン)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        #region AuthorizedUserProperty
        private User _AuthorizedUser;

        public User AuthorizedUser
        {
            get
            { return _AuthorizedUser; }
            set
            { 
                if (_AuthorizedUser == value)
                    return;
                _AuthorizedUser = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public void Initialize()
        {
            if (SessionService.Instance.IsAuthorized == false)
            {
                var message = new TransitionMessage(typeof(Views.Logon), new LogonViewModel(), TransitionMode.Modal, "ShowLogon");
                Messenger.Raise(message);
            }

            // ログオンに成功していない場合は再度ログオン画面を表示。
            //if (SessionService.Instance.IsAuthorized == false)
            //{
            //    this.Initialize();
            //}

            this.AuthorizedUser = SessionService.Instance.AuthorizedUser;
        }

        #region ShowThanksCardCreateCommand
        private ViewModelCommand _ShowThanksCardCreateCommand;

        public ViewModelCommand ShowThanksCardCreateCommand
        {
            get
            {
                if (_ShowThanksCardCreateCommand == null)
                {
                    _ShowThanksCardCreateCommand = new ViewModelCommand(ShowThanksCardCreate);
                }
                return _ShowThanksCardCreateCommand;
            }
        }

        public void ShowThanksCardCreate()
        {
            System.Diagnostics.Debug.WriteLine("ShowThanksCardCreate");
            ThanksCardCreateViewModel ViewModel = new ThanksCardCreateViewModel();
            var message = new TransitionMessage(typeof(Views.ThanksCardCreate), ViewModel, TransitionMode.Modal, "ShowThanksCardCreate");
            Messenger.Raise(message);
        }
        #endregion

        #region ShowThanksCardListCommand
        private ViewModelCommand _ShowThanksCardListCommand;

        public ViewModelCommand ShowThanksCardListCommand
        {
            get
            {
                if (_ShowThanksCardListCommand == null)
                {
                    _ShowThanksCardListCommand = new ViewModelCommand(ShowThanksCardList);
                }
                return _ShowThanksCardListCommand;
            }
        }

        public void ShowThanksCardList()
        {
            System.Diagnostics.Debug.WriteLine("ShowThanksCardList");
            ThanksCardListViewModel ViewModel = new ThanksCardListViewModel();
            var message = new TransitionMessage(typeof(Views.ThanksCardList), ViewModel, TransitionMode.Modal, "ShowThanksCardList");
            Messenger.Raise(message);
        }
        #endregion

        #region ShowUserMstCommand
        private ViewModelCommand _ShowUserMstCommand;

        public ViewModelCommand ShowUserMstCommand
        {
            get
            {
                if (_ShowUserMstCommand == null)
                {
                    _ShowUserMstCommand = new ViewModelCommand(ShowUserMst);
                }
                return _ShowUserMstCommand;
            }
        }

        public void ShowUserMst()
        {
            System.Diagnostics.Debug.WriteLine("ShowUserMst");
            UserMstViewModel ViewModel = new UserMstViewModel();
            var message = new TransitionMessage(typeof(Views.UserMst), ViewModel, TransitionMode.Modal, "ShowUserMst");
            Messenger.Raise(message);
        }
        #endregion

    }
}