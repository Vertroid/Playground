package com.rockvr.test.adapter;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentActivity;
import androidx.viewpager2.adapter.FragmentStateAdapter;

import com.rockvr.test.fragment.AbstractFragment;
import com.rockvr.test.fragment.TestAssetsHelperFragment;
import com.rockvr.test.fragment.TestDataServiceFragment;
import com.rockvr.test.fragment.TestVideoApiFragment;

import java.util.ArrayList;
import java.util.List;

public class ViewPagerAdapter extends FragmentStateAdapter {
    private final List<AbstractFragment> mFragments;

    public ViewPagerAdapter(FragmentActivity activity) {
        super(activity);

        mFragments = new ArrayList<>();
        mFragments.add(new TestVideoApiFragment());
    }

    @NonNull
    @Override
    public Fragment createFragment(int position) {
        return mFragments.get(position);
    }

    public String getFragmentTitle(int index) {
        return mFragments.get(index).getTitle();
    }

    @Override
    public int getItemCount() {
        return mFragments.size();
    }
}
