﻿using System;
using System.Collections.Generic;
using NetSql.Enums;

namespace NetSql.Pagination
{
    /// <summary>
    /// 分页类
    /// </summary>
    public class Paging
    {
        /// <summary>
        /// 创建分页对象、默认当前页为1，页大小为15
        /// </summary>
        public Paging() : this(1, 15)
        {

        }

        /// <summary>
        /// 创建分页对象
        /// </summary>
        /// <param name="index">当前页</param>
        /// <param name="size">页大小</param>
        public Paging(int index, int size)
        {
            _index = index;
            _size = size;
            OrderBy = new List<string>();
        }

        private int _index;
        private int _size;
        private int _defaultSize = 15;

        /// <summary>
        /// 页码
        /// </summary>
        public int Index
        {
            get => _index < 1 ? 1 : _index;
            set => _index = value;
        }

        /// <summary>
        /// 页大小
        /// </summary>
        public int Size
        {
            get => _size < 1 || _size > MaxSize ? _defaultSize : _size;
            set => _size = value;
        }

        /// <summary>
        /// 页大小最大值
        /// </summary>
        public int MaxSize { get; set; } = 100;

        /// <summary>
        /// 跳过数量
        /// </summary>
        public int Skip => (Index - 1) * Size;

        /// <summary>
        /// 排序字段
        /// </summary>
        public IList<string> OrderBy { get; }

        /// <summary>
        /// 排序规则
        /// </summary>
        public SortType Sort { get; set; } = SortType.Asc;

        /// <summary>
        /// 总数量
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPage => (TotalCount - 1 + Size) / Size;
    }

}
