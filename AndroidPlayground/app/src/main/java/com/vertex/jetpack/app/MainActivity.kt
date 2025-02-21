package com.vertex.jetpack.app

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Column
import androidx.compose.material3.Button
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.tooling.preview.Preview
import com.vertex.plugin.utils.FFmpegUtils

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            Column {

                Text("Vertex API Tester")
                MessageCard("Vertex")
                Button(onClick = {
                    FFmpegUtils.testCheckProtocol()
                }) {
                    Text("Test FFmpeg")
                }
            }
        }
    }
}

@Composable
fun MessageCard(name: String) {
    Text(text = "Tester: $name")
}

@Preview
@Composable
fun PreviewMessageCard() {
    MessageCard("Vertex")
}
