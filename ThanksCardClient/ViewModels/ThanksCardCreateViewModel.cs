﻿#nullable disable
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThanksCardClient.Models;
using ThanksCardClient.Services;

namespace ThanksCardClient.ViewModels
{
    public class ThanksCardCreateViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;

        #region ThanksCardProperty
        private ThanksCard _ThanksCard;
        public ThanksCard ThanksCard
        {
            get { return _ThanksCard; }
            set { SetProperty(ref _ThanksCard, value); }
        }
        #endregion

        #region FromUsersProperty
        private List<User> _FromUsers;
        public List<User> FromUsers
        {
            get { return _FromUsers; }
            set { SetProperty(ref _FromUsers, value); }
        }
        #endregion

        #region ToUsersProperty
        private List<User> _ToUsers;
        public List<User> ToUsers
        {
            get { return _ToUsers; }
            set { SetProperty(ref _ToUsers, value); }
        }
        #endregion

        #region SearchThanksCardProperty
        private SearchThanksCard _SearchThanksCard;
        public SearchThanksCard SearchThanksCard
        {
            get { return _SearchThanksCard; }
            set { SetProperty(ref _SearchThanksCard, value); }
        }
        #endregion

        #region DepartmentsProperty
        private List<Department> _Departments;
        public List<Department> Departments
        {
            get { return _Departments; }
            set { SetProperty(ref _Departments, value); }
        }
        #endregion

        #region TagsProperty
        private List<Tag> _Tags;
        public List<Tag> Tags
        {
            get { return _Tags; }
            set { SetProperty(ref _Tags, value); }
        }
        #endregion

        public ThanksCardCreateViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            this.AuthorizedUser = SessionService.Instance.AuthorizedUser;
            this._SearchWord = this.AuthorizedUser.Name;
        }
        #region roginuser
        private User _AuthorizedUser;
        public User AuthorizedUser
        {
            get { return _AuthorizedUser; }
            set { SetProperty(ref _AuthorizedUser, value); }
        }
        #endregion
        #region LoginUserProperty
        private string _LoginUser;
        public string LoginUser
        {
            get { return _LoginUser; }
            set
            {
                SetProperty(ref _LoginUser, value);
                //    System.Diagnostics.Debug.WriteLine("SearchWord: " + this.SearchWord); //動作確認用。本来はこの行は必要ありません。
            }
        }
        #endregion

        #region SearchWordProperty
        private string _SearchWord;
        public string SearchWord
        {
            get { return _SearchWord; }
            set
            {
                SetProperty(ref _SearchWord, value);
                System.Diagnostics.Debug.WriteLine("SearchWord: " + this.SearchWord); //動作確認用。本来はこの行は必要ありません。
            }
        }
        #endregion

        // この画面に遷移し終わったときに呼ばれる。
        // それを利用し、画面表示に必要なプロパティを初期化している。
        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.ThanksCard = new ThanksCard();
            
            if (SessionService.Instance.AuthorizedUser != null)
            {
                this.FromUsers = await SessionService.Instance.AuthorizedUser.GetUsersAsync();
                this.ToUsers = this.FromUsers;
            }

            var tag = new Tag();
            this.Tags = await tag.GetTagsAsync();

            var dept = new Department();
            this.Departments = await dept.GetDepartmentsAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        #region FromDepartmentsChangedCommand
        private DelegateCommand<long?> _FromDepartmentsChangedCommand;
        public DelegateCommand<long?> FromDepartmentsChangedCommand =>
            _FromDepartmentsChangedCommand ?? (_FromDepartmentsChangedCommand = new DelegateCommand<long?>(ExecuteFromDepartmentsChangedCommand));

        async void ExecuteFromDepartmentsChangedCommand(long? FromDepartmentId)
        {
            this.FromUsers = await SessionService.Instance.AuthorizedUser.GetDepartmentUsersAsync(FromDepartmentId);
        }
        #endregion

        #region ToDepartmentsChangedCommand
        private DelegateCommand<long?> _ToDepartmentsChangedCommand;
        public DelegateCommand<long?> ToDepartmentsChangedCommand =>
            _ToDepartmentsChangedCommand ?? (_ToDepartmentsChangedCommand = new DelegateCommand<long?>(ExecuteToDepartmentsChangedCommand));

        async void ExecuteToDepartmentsChangedCommand(long? ToDepartmentId)
        {
            this.ToUsers = await SessionService.Instance.AuthorizedUser.GetDepartmentUsersAsync(ToDepartmentId);
        }
        #endregion

        #region SubmitCommand
        private DelegateCommand _SubmitCommand;
        public DelegateCommand SubmitCommand =>
            _SubmitCommand ?? (_SubmitCommand = new DelegateCommand(ExecuteSubmitCommand));

        async void ExecuteSubmitCommand()
        {
            System.Diagnostics.Debug.WriteLine(this.Tags);

            //選択された Tag を取得し、ThanksCard.ThanksCardTags にセットする。
            List<ThanksCardTag> ThanksCardTags = new List<ThanksCardTag>();
            foreach (var tag in this.Tags.Where(t => t.Selected))
            {
                ThanksCardTag thanksCardTag = new ThanksCardTag();
                thanksCardTag.TagId = tag.Id;
                ThanksCardTags.Add(thanksCardTag);
            }
            this.ThanksCard.ThanksCardTags = ThanksCardTags;

            ThanksCard createdThanksCard = await ThanksCard.PostThanksCardAsync(this.ThanksCard);

            //TODO: Error handling
            this.regionManager.RequestNavigate("ContentRegion", nameof(Views.ThanksCardList));

        }
        #endregion
       
        #region HomeCommand
        private DelegateCommand _HomeCommand;
        public DelegateCommand HomeCommand =>
            _HomeCommand ?? (_HomeCommand = new DelegateCommand(ExecuteHomeCommand));

        void ExecuteHomeCommand()
        {
            this.regionManager.RequestNavigate("ContentRegion", nameof(Views.MenuUser));
        }
        #endregion

        #region HitCommand
        private DelegateCommand _HitCommand;
        public DelegateCommand HitCommand =>
            _HitCommand ?? (_HitCommand = new DelegateCommand(ExecuteHitCommand));

        void ExecuteHitCommand()
        {
            this.regionManager.RequestNavigate("ContentRegion", nameof(Views.Hit));
        }
        #endregion

        #region  ThanksCradCreateCommand
        private DelegateCommand _ThanksCradCreateCommand;


        public DelegateCommand ThanksCradCreateCommand =>
            _ThanksCradCreateCommand ?? (_ThanksCradCreateCommand = new DelegateCommand(ExecuteThanksCradCreateCommand));

        void ExecuteThanksCradCreateCommand()
        {
            this.regionManager.RequestNavigate("ContentRegion", nameof(Views.ThanksCardCreate));
        }
        #endregion

        #region  MenuUserCommand
        private DelegateCommand _MenuUserCommand;


        public DelegateCommand MenuUserCommand =>
            _MenuUserCommand ?? (_MenuUserCommand = new DelegateCommand(ExecuteMenuUserCommand));

        void ExecuteMenuUserCommand()
        {
            this.regionManager.RequestNavigate("ContentRegion", nameof(Views.MenuUser));
        }
        #endregion

        #region  MenuUser1Command
        private DelegateCommand _MenuUser1Command;


        public DelegateCommand MenuUser1Command =>
            _MenuUser1Command ?? (_MenuUser1Command = new DelegateCommand(ExecuteMenuUser1Command));

        void ExecuteMenuUser1Command()
        {
            this.regionManager.RequestNavigate("ContentRegion", nameof(Views.MenuUser));
        }
        #endregion

        #region  ThanksCradListCommand
        private DelegateCommand _ThanksCradListCommand;


        public DelegateCommand ThanksCradListCommand =>
            _ThanksCradListCommand ?? (_MenuUserCommand = new DelegateCommand(ExecuteThanksCradListCommand));

        void ExecuteThanksCradListCommand()
        {
            this.regionManager.RequestNavigate("ContentRegion", nameof(Views.ThanksCardList));
        }
        #endregion

    }
}
