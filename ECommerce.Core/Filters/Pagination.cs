namespace ECommerce.Core.Filters
{
    public class Pagination
    {
        int _page = 0;
        int _size = 5;
        public int Page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value < 0 ? 0 : value;
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value <= 0 ? 5 : (value > 100 ? 100 : value);
            }
        }
    }
}
