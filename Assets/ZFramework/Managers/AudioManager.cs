using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{

    public class AudioManager : BaseManager//TODO ...侧烂污赶时间
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Audio; } }

        [SerializeField] AudioSource mBackGroundSource = null;
        [SerializeField] AudioSource mSoundSource = null;

        internal override void Init()
        {
            Z.Debug.Log("AudioManager init");
            Z.Audio = this;
            mBackGroundSource.loop = true;
            mSoundSource.loop = false;
        }

        public void PlayBackGroundMusic(string path)
        {
            var clip = Z.Resource.LoadResource<AudioClip>(path, (int)BuiltinGroup.Audio);
            mBackGroundSource.clip = clip;
            mBackGroundSource.Play();
        }
        public void PlaySound(string path)
        {
            var clip = Z.Resource.LoadResource<AudioClip>(path, (int)BuiltinGroup.Audio);
            mSoundSource.clip = clip;
            mSoundSource.Play();
        }
        public void Release()
        {
            Z.Resource.Release((int)BuiltinGroup.Audio);
        }

        internal override void MgrUpdate()
        {
           
        }

        internal override void ShutDown()
        {
            throw new NotImplementedException();
        }
    }
}