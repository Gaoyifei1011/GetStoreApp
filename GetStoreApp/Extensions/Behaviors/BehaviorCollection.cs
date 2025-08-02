using Microsoft.UI.Xaml;
using System.Collections.Generic;
using Windows.Foundation.Collections;

namespace GetStoreApp.Extensions.Behaviors
{
    /// <summary>
    /// 行为集合
    /// </summary>
    public sealed partial class BehaviorCollection : DependencyObjectCollection
    {
        private readonly List<IBehavior> oldBehaviorList = [];

        /// <summary>
        /// 获取附加到行为集合的依赖对象
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

        /// <summary>
        /// 初始化行为集合类的新实例
        /// </summary>
        public BehaviorCollection()
        {
            VectorChanged += (sender, args) =>
            {
                if (args.CollectionChange is CollectionChange.Reset)
                {
                    foreach (IBehavior oldBehavior in oldBehaviorList)
                    {
                        if (oldBehavior.AssociatedObject is not null)
                        {
                            oldBehavior.Detach();
                        }
                    }

                    oldBehaviorList.Clear();
                    using IEnumerator<DependencyObject> enumerator2 = GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        DependencyObject current2 = enumerator2.Current;
                        oldBehaviorList.Add(VerifiedAttach(current2));
                    }

                    return;
                }

                int index = (int)args.Index;
                DependencyObject dependencyObject = base[index];
                switch (args.CollectionChange)
                {
                    case CollectionChange.ItemInserted:
                        {
                            oldBehaviorList.Insert(index, VerifiedAttach(dependencyObject));
                            break;
                        }
                    case CollectionChange.ItemChanged:
                        {
                            IBehavior behavior = oldBehaviorList[index];
                            if (behavior.AssociatedObject is not null)
                            {
                                behavior.Detach();
                            }

                            oldBehaviorList[index] = VerifiedAttach(dependencyObject);
                            break;
                        }
                    case CollectionChange.ItemRemoved:
                        {
                            IBehavior behavior = oldBehaviorList[index];
                            if (behavior.AssociatedObject is not null)
                            {
                                behavior.Detach();
                            }

                            oldBehaviorList.RemoveAt(index);
                            break;
                        }
                }
            };
        }

        /// <summary>
        /// 将行为集合附加到指定的依赖对象
        /// </summary>
        /// <param name="associatedObject">要附加到的依赖对象</param>
        public void Attach(DependencyObject associatedObject)
        {
            if (!Equals(associatedObject, AssociatedObject) && AssociatedObject is null)
            {
                AssociatedObject = associatedObject;
                using IEnumerator<DependencyObject> enumerator = GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ((IBehavior)enumerator.Current).Attach(AssociatedObject);
                }
            }
        }

        /// <summary>
        /// 从关联对象中分离行为集合
        /// </summary>
        public void Detach()
        {
            using (IEnumerator<DependencyObject> enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    IBehavior behavior = (IBehavior)enumerator.Current;
                    if (behavior.AssociatedObject is not null)
                    {
                        behavior.Detach();
                    }
                }
            }

            AssociatedObject = null;
            oldBehaviorList.Clear();
        }

        private IBehavior VerifiedAttach(DependencyObject dependencyObject)
        {
            if (dependencyObject is IBehavior behavior && !oldBehaviorList.Contains(behavior) && AssociatedObject is not null)
            {
                behavior.Attach(AssociatedObject);
                return behavior;
            }
            else
            {
                return null;
            }
        }
    }
}
