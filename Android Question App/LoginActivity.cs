using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android_Question_App.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Android_Question_App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {
        RecyclerView reditRecylerView;
        Button searchButton;
        RecyclerView.LayoutManager mLayoutManager;
        ProgressBar progressBarRequesting;
        TextInputEditText textInputEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            reditRecylerView = FindViewById<RecyclerView>(Resource.Id.reditRecylerView);

            progressBarRequesting = FindViewById<ProgressBar>(Resource.Id.progressBarReditLoading);

            textInputEditText = FindViewById<TextInputEditText>(Resource.Id.textInput1);

            searchButton = FindViewById<Button>(Resource.Id.search_button);
            searchButton.Click += onSearchButtonClicked;
        }

        private void onSearchButtonClicked(object sender, EventArgs e)
        {
            hideKeyboard();
            string query = textInputEditText.Text;
            if (query == null || query.Trim().Equals("")) {
                return;
            }
            
            searchButton.Enabled = false;
            searchButton.Text = GetString(Resource.String.searching);
            ThreadPool.QueueUserWorkItem(async o => await fetchSubRedditsAsync(query));
        }

        private async Task fetchSubRedditsAsync(String query)
        {
            RunOnUiThread(() =>
            {
                reditRecylerView.Visibility = ViewStates.Gone;
                progressBarRequesting.Visibility = ViewStates.Visible;
            });

            using WebClient client = new WebClient();
            {
                try
                {
                    string json = await client.DownloadStringTaskAsync(new Uri("http://www.reddit.com/subreddits/search.json?q=" + query)).ConfigureAwait(false);

                    JObject subreddits = JsonConvert.DeserializeObject<JObject>(json);

                    RunOnUiThread(() =>
                    {
                        reditRecylerView.RemoveAllViews();

                        List<ReditItem> reditItems = new List<ReditItem>();

                        foreach (JToken subreddit in subreddits["data"]["children"] as JArray)
                        {
                            string name = subreddit["data"]["display_name_prefixed"].ToString();

                            reditItems.Add(new ReditItem
                            {
                                Name = name
                            });
                        }

                        mLayoutManager = new LinearLayoutManager(this);
                        reditRecylerView.SetLayoutManager(mLayoutManager);

                        ReditListAdapter mAdapter = new ReditListAdapter(reditItems);

                        reditRecylerView.SetAdapter(mAdapter);

                        mAdapter.ItemClick += OnItemClick;

                        progressBarRequesting.Visibility = ViewStates.Gone;
                        reditRecylerView.Visibility = ViewStates.Visible;

                        searchButton.Enabled = true;
                        searchButton.Text = GetString(Resource.String.search);

                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void OnItemClick(object sender, ReditItem e)
        {
            ReditItem reditItem = (ReditItem)e;
            var sidebarUrl = "http://www.reddit.com/" + reditItem.Name + "/about/sidebar";
            var intent = new Intent(this, typeof(SidebarActivity));
            intent.PutExtra("sidebarUrl", sidebarUrl);
            this.StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void hideKeyboard() {
            var textInput = FindViewById<TextInputEditText>(Resource.Id.textInput1);
            textInput.Enabled = false;
            textInput.Enabled = true;
        }
    }
}

