using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestAsyncAwait : MonoBehaviour, IPointerEnterHandler
{
    void Start()
    {
        Debug.Log($"Start Thread ID: {Thread.CurrentThread.ManagedThreadId}");
    }

    public async void OnPointerEnter(PointerEventData eventData)
    {
        TestPointerEnterAsync();
    }

    private async void TestPointerEnterSync()
    {
        Thread.Sleep(1000);
        Debug.Log($"Sync Thread ID: {Thread.CurrentThread.ManagedThreadId}");
    }

    private async void TestPointerEnterAsync()
    {
        await Task.Run(() =>
        {
            Thread.Sleep(1000);
            Debug.Log($"Async Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        });
    }

}
