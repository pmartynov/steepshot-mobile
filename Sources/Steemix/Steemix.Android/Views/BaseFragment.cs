using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Steemix.Droid.ViewModels;

namespace Steemix.Droid.Views
{
	public class BaseFragment<T> : Fragment where T : MvvmViewModelBase
	{
		protected T ViewModel { get { return SteemixApp.ViewModelLocator.GetViewModel<T>(); } }

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			ViewModel.ViewLoad();
		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();
		}

		public override void OnResume()
		{
			base.OnResume();
			ViewModel.ViewAppear();
		}

		public override void OnPause()
		{
			ViewModel.ViewDisappear();
			base.OnPause();
		}
	}
}
