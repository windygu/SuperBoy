using System;
using System.Security.Cryptography;

namespace Core.Web.CacheManage.Memcached
{
	
	public class Fnv132 : HashAlgorithm
	{
		private static readonly uint FnvPrime = 16777619;
		private static readonly uint OffsetBasis = 2166136261;

		protected uint hash;

		public Fnv132() {
			HashSizeValue = 32;
		}

		public override void Initialize() {
			hash = OffsetBasis;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize) {
			var length = ibStart + cbSize;
			for (var i = ibStart; i < length; i++) {
				hash = (hash * FnvPrime)^array[i];
			}
		}

		protected override byte[] HashFinal() {
			return BitConverter.GetBytes(hash);
		}
	}

	/// <summary>
	/// Fowler-Noll-Vo hash, variant 1a, 32-bit version.
	/// http://www.isthe.com/chongo/tech/comp/fnv/
	/// </summary>
	public class Fnv1A32 : HashAlgorithm
	{
		private static readonly uint FnvPrime = 16777619;
		private static readonly uint OffsetBasis = 2166136261;

		protected uint hash;

		public Fnv1A32() {
			HashSizeValue = 32;
		}

		public override void Initialize() {
			hash = OffsetBasis;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize) {
			var length = ibStart + cbSize;
			for (var i = ibStart; i < length; i++) {
				hash = (hash^array[i]) * FnvPrime;
			}
		}

		protected override byte[] HashFinal() {
			return BitConverter.GetBytes(hash);
		}
	}

	/// <summary>
	/// Modified Fowler-Noll-Vo hash, 32-bit version.
	/// http://home.comcast.net/~bretm/hash/6.html
	/// </summary>
	public class ModifiedFnv132 : Fnv132
	{
		protected override byte[] HashFinal() {
			hash += hash << 13;
			hash ^= hash >> 7;
			hash += hash << 3;
			hash ^= hash >> 17;
			hash += hash << 5;
			return BitConverter.GetBytes(hash);
		}
	}
}