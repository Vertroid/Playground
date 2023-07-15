package com.rockvr.test.activity;

import android.os.Bundle;
import android.util.Log;

import androidx.appcompat.app.AppCompatActivity;
import androidx.viewpager2.widget.ViewPager2;

import com.google.android.material.tabs.TabLayout;
import com.google.android.material.tabs.TabLayoutMediator;
import com.rockvr.test.R;
import com.rockvr.test.adapter.ViewPagerAdapter;
import com.vertex.plugin.jni.NativePlugin;


public class MainActivity extends AppCompatActivity {
    private String TAG = "VERTEX";
    private ViewPagerAdapter mPagerAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        init();
    }

    private void init() {
        TabLayout tabLayout = findViewById(R.id.tabLayout);
        ViewPager2 viewPager = findViewById(R.id.viewPager);
        mPagerAdapter = new ViewPagerAdapter(this);

        viewPager.setAdapter(mPagerAdapter);

        new TabLayoutMediator(tabLayout, viewPager, (tab, position) -> {
            tab.setText(mPagerAdapter.getFragmentTitle(position));
        }).attach();

        Log.e(TAG, NativePlugin.hello());
    }
}
