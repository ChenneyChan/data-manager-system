namespace ABBDataManagerSystem.Bean.Base { 

    public interface BeanBase
    {
        public static List<BeanBase>? GetFromDB(bool filterTemplate = false);
    }
}
