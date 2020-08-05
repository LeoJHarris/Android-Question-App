using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android_Question_App.Models;

namespace Android_Question_App
{
    public class ReditListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ReditItem> ItemClick;

        readonly List<ReditItem> _reditItem;

        public ReditListAdapter(List<ReditItem> reditItem)
        {
            _reditItem = reditItem;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.redit_view, parent, false);

            return new ReditViewHolder(itemView, OnClick);
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ReditViewHolder vh = holder as ReditViewHolder;

            vh.Name.Text = _reditItem[position].Name;
        }

        public override int ItemCount
        {
            get { return _reditItem.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, _reditItem[position]);
        }
    }

    public class ReditViewHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; private set; }

        public ReditViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            Name = itemView.FindViewById<TextView>(Resource.Id.reditNameTextView);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}