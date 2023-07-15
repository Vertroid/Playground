package com.rockvr.test.fragment;

import android.content.res.AssetManager;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.rockvr.test.R;

import java.io.IOException;
import java.io.InputStream;

public class TestAssetsHelperFragment extends AbstractFragment {
    private final String mTag = "Assets";
    private final String mTargetFileName = "src/main/scene.zip";

    public TestAssetsHelperFragment() {
        super();
        mTitle = "SCENE";
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_scene, container, false);

        Button checkButton = view.findViewById(R.id.checkButton);
        checkButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                checkIfSceneExists();
            }
        });

        return view;
    }

    public boolean checkIfSceneExists() {
        AssetManager manager = getActivity().getAssets();
        try {
            String[] names = manager.list("");
            for(int i = 0; i < names.length; i++) {
                Log.d("Assets", names[i]);

                if(names[i].equals(mTargetFileName.trim())) {
                    Log.d(mTag, "Exists");

                    InputStream is = manager.open(names[i]);

                }
            }
        } catch (IOException ioe) {

        }
        return true;
    }
}
