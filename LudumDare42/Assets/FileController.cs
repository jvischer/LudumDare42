using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FileController : MonoBehaviour {

    [SerializeField] private string _selectedTrigger = "";
    [SerializeField] private string _deselectedTrigger = "";

    private Animator _animator;

    private void Awake() {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable() {
        FileSystemManager.FSM.registerFile(this);
    }

    private void OnDisable() {
        FileSystemManager.FSM.deregisterFile(this);
    }

    public void trySelectFile() {
        _animator.SetTrigger(_selectedTrigger);
    }

    public void tryDeselectFile() {
        _animator.SetTrigger(_deselectedTrigger);
    }

}
